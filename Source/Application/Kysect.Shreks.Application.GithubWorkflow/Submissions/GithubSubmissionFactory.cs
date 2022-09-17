using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Specifications.Submissions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Kysect.CommonLib;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Specifications.GroupAssignments;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Models;

namespace Kysect.Shreks.Application.GithubWorkflow.Submissions;

public class GithubSubmissionFactory : IGithubSubmissionFactory
{
    private readonly IShreksDatabaseContext _context;

    public GithubSubmissionFactory(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<GithubSubmissionCreationResult> CreateOrUpdateGithubSubmission(
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pullRequestDescriptor);

        Guid userId = await GetUserId(pullRequestDescriptor, cancellationToken);
        Guid assignmentId = await GetAssignmentId(pullRequestDescriptor, cancellationToken);
        bool triggeredByMentor = await PermissionValidator.IsOrganizationMentor(_context, userId, pullRequestDescriptor.Organization);
        bool triggeredByAnotherUser = !PermissionValidator.IsRepositoryOwner(pullRequestDescriptor.Sender, pullRequestDescriptor.Repository);

        var submissionSpec = new FindLatestGithubSubmission(
            pullRequestDescriptor.Organization,
            pullRequestDescriptor.Repository,
            pullRequestDescriptor.PrNumber);

        var submission = await _context.SubmissionAssociations
            .WithSpecification(submissionSpec)
            .Where(x => x.State != SubmissionState.Completed)
            .FirstOrDefaultAsync(cancellationToken);

        bool isCreated = false;

        if (submission is null || submission.IsRated)
        {
            if (triggeredByAnotherUser && !triggeredByMentor)
            {
                var message = $"Repository {pullRequestDescriptor.Repository} is assigned to another student. " +
                              $"User {pullRequestDescriptor.Sender} does not have permission here. Close this PR and open new with correct account.";
                throw new DomainInvalidOperationException(message);
            }

            submission = await CreateGithubSubmissionAsync(
                userId,
                assignmentId,
                pullRequestDescriptor,
                cancellationToken);
            isCreated = true;

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else if (!triggeredByMentor)
        {
            submission.SubmissionDate = Calendar.CurrentDateTime;

            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync(cancellationToken);

            if (triggeredByAnotherUser)
            {
                var message = $"Repository {pullRequestDescriptor.Repository} is assigned to another student. " +
                              $"Do not use {pullRequestDescriptor.Sender} account for this repository. Submission date will be updated.";
                throw new DomainInvalidOperationException(message);
            }
        }

        return new GithubSubmissionCreationResult(submission, isCreated);
    }

    private async Task<Guid> GetUserId(GithubPullRequestDescriptor pullRequestDescriptor, CancellationToken cancellationToken)
    {
        User user = await _context.UserAssociations.GetUserByGithubUsername(pullRequestDescriptor.Sender);

        return user.Id;
    }

    private async Task<Guid> GetAssignmentId(GithubPullRequestDescriptor pullRequestDescriptor, CancellationToken cancellationToken)
    {
        var subjectCourseAssociation = await _context.SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(gsc => gsc.GithubOrganizationName == pullRequestDescriptor.Organization, cancellationToken);

        if (subjectCourseAssociation is null)
            throw new EntityNotFoundException($"SubjectCourse with organization {pullRequestDescriptor.Organization} not found");

        var assignment = subjectCourseAssociation.SubjectCourse.Assignments.FirstOrDefault(a => a.ShortName == pullRequestDescriptor.BranchName);

        if (assignment is null)
        {
            var branchName = pullRequestDescriptor.BranchName;
            string assignments = subjectCourseAssociation.SubjectCourse
                .Assignments
                .OrderBy(a => a.Order)
                .ToSingleString(a => a.ShortName);

            var message = $"Assignment with branch name '{branchName}' for subject course '{subjectCourseAssociation.SubjectCourse.Title}' was not found." +
                          $"\nEnsure that branch name is correct. Available assignments: {assignments}";
            throw new EntityNotFoundException(message);
        }

        return assignment.Id;
    }

    public async Task<GithubSubmission> CreateGithubSubmissionAsync(
        Guid userId,
        Guid assignmentId,
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken)
    {
        var student = await _context.Students.FindAsync(new object[] { userId }, cancellationToken);
        if (student is null)
        {
            var courseAssociation = await _context.SubjectCourseAssociations
                .OfType<GithubSubjectCourseAssociation>()
                .Include(association => association.SubjectCourse)
                .SingleOrDefaultAsync(x => x.GithubOrganizationName == pullRequestDescriptor.Organization, cancellationToken)
                ?? throw new EntityNotFoundException(
                    $"Course association was not found for organization {pullRequestDescriptor.Organization}");

            _ = await _context.Mentors.FindAsync(new object[] { userId, courseAssociation.SubjectCourse.Id }, cancellationToken)
                ?? throw new EntityNotFoundException(
                    $"User with id {userId} not found or have no permissions to create submission");

            student = await FindStudentByRepositoryName(pullRequestDescriptor, cancellationToken);
        }

        var groupAssignmentSpec = new GetStudentGroupAssignment(student.UserId, assignmentId);

        var groupAssignment = await _context.GroupAssignments
            .WithSpecification(groupAssignmentSpec)
            .SingleAsync(cancellationToken);

        var studentAssignmentSubmissionsSpec = new GetStudentAssignmentSubmissions(
            student.UserId, assignmentId);

        var count = await _context.Submissions
            .WithSpecification(studentAssignmentSubmissionsSpec)
            .CountAsync(cancellationToken);

        var submission = new GithubSubmission
        (
            count + 1,
            student,
            groupAssignment,
            Calendar.CurrentDateTime,
            pullRequestDescriptor.Payload,
            pullRequestDescriptor.Organization,
            pullRequestDescriptor.Repository,
            pullRequestDescriptor.PrNumber
        );

        return submission;
    }

    private async Task<Student> FindStudentByRepositoryName(
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken)
    {
        User user = await _context.UserAssociations.GetUserByGithubUsername(pullRequestDescriptor.Repository);

        return await _context.Students.GetByIdAsync(user.Id, cancellationToken);
    }
}