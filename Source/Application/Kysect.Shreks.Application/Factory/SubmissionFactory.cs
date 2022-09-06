using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.GroupAssignments;
using Kysect.Shreks.Core.Specifications.Submissions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Factory;

public class SubmissionFactory : ISubmissionFactory
{
    private readonly IShreksDatabaseContext _context;

    public SubmissionFactory(IShreksDatabaseContext context)
    {
        _context = context;
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
        var association = await _context.UserAssociations
                              .OfType<GithubUserAssociation>()
                              .Include(x => x.User)
                              .SingleOrDefaultAsync(x => x.GithubUsername == pullRequestDescriptor.Repository, cancellationToken)
                          ?? throw new EntityNotFoundException($"Unable to find student by GithubUserAssociation for {pullRequestDescriptor.Repository} repository");

        return await _context.Students.GetByIdAsync(association.User.Id, cancellationToken);
    }
}