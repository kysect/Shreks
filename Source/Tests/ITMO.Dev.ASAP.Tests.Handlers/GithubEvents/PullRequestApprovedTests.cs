using FluentAssertions;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Contracts.GithubEvents;
using ITMO.Dev.ASAP.Application.Handlers.GithubEvents;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Submissions.States;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.GithubEvents;

public class PullRequestApprovedTests : GithubEventsTestBase
{
    [Fact]
    public async Task Handle_ShouldMarkSubmissionAsReviewed_WhenApprovedByMentor()
    {
        Submission submission = await Context.GetGithubSubmissionAsync(new ActiveSubmissionState());
        Mentor issuer = submission.GetMentor();
        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestApproved.Command(issuer.UserId, submission.Id);
        var handler = new PullRequestApprovedHandler(workflowService);

        await handler.Handle(command, CancellationToken.None);

        submission.State.Should().BeOfType<ReviewedSubmissionState>();
        submission.Rating.Should().BeNull();
        submission.ExtraPoints.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenApprovedByNotMentor()
    {
        Submission submission = await Context.GetGithubSubmissionAsync(new ActiveSubmissionState());
        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();
        User issuer = await Context.GetNotMentorUser(submission.GroupAssignment, submission.Student.User);

        var command = new PullRequestApproved.Command(issuer.Id, submission.Id);
        var handler = new PullRequestApprovedHandler(workflowService);

        Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedException>();
    }
}