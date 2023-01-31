using FluentAssertions;
using Kysect.Shreks.Application.Contracts.Github.Queries;
using Kysect.Shreks.Application.Handlers.Github.Submissions;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers.Github;

public class GetLastPullRequestSubmissionHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_Should_NotThrow()
    {
        Submission submission = await Context.Submissions
            .Where(x => x.Associations.OfType<GithubSubmissionAssociation>().Any())
            .Where(x =>
                x.GroupAssignment.Assignment.SubjectCourse.WorkflowType ==
                SubmissionStateWorkflowType.ReviewWithDefense)
            .OrderByDescending(x => x.Code)
            .FirstAsync();

        Guid userId = submission.Student.UserId;
        Guid assignmentId = submission.GroupAssignment.Assignment.Id;

        long pullRequestNumber = submission.Associations
            .OfType<GithubSubmissionAssociation>()
            .First()
            .PrNumber;

        var query = new GetLastPullRequestSubmission.Query(userId, assignmentId, pullRequestNumber);
        var handler = new GetLastPullRequestSubmissionHandler(Context);

        GetLastPullRequestSubmission.Response response = await handler.Handle(query, default);

        response.Submission.Id.Should().Be(submission.Id);
    }
}