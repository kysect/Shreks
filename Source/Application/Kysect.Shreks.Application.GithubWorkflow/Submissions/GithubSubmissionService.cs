using Kysect.CommonLib;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.GithubWorkflow.Submissions;

public class GithubSubmissionService : IGithubSubmissionService
{
    private readonly IShreksDatabaseContext _context;

    public GithubSubmissionService(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Submission> GetLastSubmissionByPr(GithubPullRequestDescriptor pullRequestDescriptor)
    {
        Submission? submission = await _context.SubmissionAssociations
            .OfType<GithubSubmissionAssociation>()
            .Where(a =>
                a.Organization == pullRequestDescriptor.Organization
                && a.Repository == pullRequestDescriptor.Repository
                && a.PrNumber == pullRequestDescriptor.PrNumber)
            .OrderByDescending(a => a.Submission.SubmissionDate)
            .Select(a => a.Submission)
            .FirstOrDefaultAsync();

        if (submission is null)
        {
            var message = $"No submission in pr {pullRequestDescriptor.Payload}";
            throw new EntityNotFoundException(message);
        }

        return submission;
    }

    public async Task<Assignment> GetAssignmentByBranchAndSubjectCourse(Guid subjectCourseId, GithubPullRequestDescriptor pullRequestDescriptor, CancellationToken cancellationToken)
    {
        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(subjectCourseId, cancellationToken);

        var assignment = subjectCourse.Assignments.FirstOrDefault(a => a.ShortName == pullRequestDescriptor.BranchName);

        if (assignment is null)
        {
            var branchName = pullRequestDescriptor.BranchName;
            string assignments = subjectCourse
                .Assignments
                .OrderBy(a => a.Order)
                .ToSingleString(a => a.ShortName);

            var message = $"Assignment with branch name '{branchName}' for subject course '{subjectCourse.Title}' was not found." +
                          $"\nEnsure that branch name is correct. Available assignments: {assignments}";
            throw new EntityNotFoundException(message);
        }

        return assignment;
    }

    public async Task<Submission> GetCurrentUnratedSubmissionByPrNumber(GithubPullRequestDescriptor pullRequestDescriptor, CancellationToken cancellationToken)
    {
        var submission = await _context.SubmissionAssociations
            .OfType<GithubSubmissionAssociation>()
            .Where(a =>
                a.Organization == pullRequestDescriptor.Organization
                && a.Repository == pullRequestDescriptor.Repository
                && a.PrNumber == pullRequestDescriptor.PrNumber
                && a.Submission.Rating == null)
            .OrderByDescending(a => a.Submission.SubmissionDate)
            .Select(a => a.Submission)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null)
        {
            var message = $"No unrated submission in pr {pullRequestDescriptor.Payload}";
            throw new EntityNotFoundException(message);
        }

        return submission;
    }
}