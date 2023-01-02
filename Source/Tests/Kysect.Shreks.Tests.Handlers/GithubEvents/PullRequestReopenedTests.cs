using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Contracts.GithubEvents;
using Kysect.Shreks.Application.Handlers.GithubEvents;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Submissions.States;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers.GithubEvents;

public class PullRequestReopenedTests : GithubEventsTestBase
{
    [Fact]
    public async Task Handle_ShouldActivateDeactivatedTask_WhenPullRequestReopened()
    {
        Submission submission = await Context.GetGithubSubmissionAsync(new InactiveSubmissionState());
        User issuer = submission.Student.User;
        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestReopened.Command(issuer.Id, submission.Id);
        var handler = new PullRequestReopenedHandler(workflowService);

        await handler.Handle(command, CancellationToken.None);

        submission.State.Should().BeOfType<ActiveSubmissionState>();
        submission.Rating.Should().BeNull();
        submission.ExtraPoints.Should().BeNull();
    }
}