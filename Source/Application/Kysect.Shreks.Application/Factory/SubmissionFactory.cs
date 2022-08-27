using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.GroupAssignments;
using Kysect.Shreks.Core.Specifications.Submissions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;
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
        var student = await _context.Students.GetByIdAsync(userId, cancellationToken);
        var groupAssignmentSpec = new GetStudentGroupAssignment(student.Id, assignmentId);

        var groupAssignment = await _context.GroupAssignments
            .WithSpecification(groupAssignmentSpec)
            .SingleAsync(cancellationToken);

        var studentAssignmentSubmissionsSpec = new GetStudentAssignmentSubmissions(
            student.Id, assignmentId);

        var count = await _context.Submissions
            .WithSpecification(studentAssignmentSubmissionsSpec)
            .CountAsync(cancellationToken);

        var submission = new GithubSubmission
        (
            count + 1,
            student,
            groupAssignment,
            Calendar.CurrentDate,
            pullRequestDescriptor.Payload,
            pullRequestDescriptor.Organization,
            pullRequestDescriptor.Repository,
            pullRequestDescriptor.PrNumber
        );

        return submission;
    }
}