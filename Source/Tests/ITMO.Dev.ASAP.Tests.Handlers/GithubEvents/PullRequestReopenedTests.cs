using FluentAssertions;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Contracts.GithubEvents;
using ITMO.Dev.ASAP.Application.Handlers.GithubEvents;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Submissions.States;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.GithubEvents;

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