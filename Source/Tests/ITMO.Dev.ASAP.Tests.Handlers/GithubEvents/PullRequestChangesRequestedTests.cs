using FluentAssertions;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Contracts.GithubEvents;
using ITMO.Dev.ASAP.Application.Handlers.GithubEvents;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Submissions.States;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Core.ValueObject;
using ITMO.Dev.ASAP.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.GithubEvents;

public class PullRequestChangesRequestedTests : GithubEventsTestBase
{
    [Fact]
    public async Task Handle_ShouldBeRatedZero_WhenIssuedByMentor()
    {
        Submission submission = await Context.GetGithubSubmissionAsync(new ActiveSubmissionState());
        Mentor issuer = submission.GetMentor();
        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestChangesRequested.Command(issuer.UserId, submission.Id);
        var handler = new PullRequestChangesRequestedHandler(workflowService);

        await handler.Handle(command, default);

        submission.State.Should().BeOfType<CompletedSubmissionState>();
        submission.Rating.Should().Be(Fraction.None);
        submission.ExtraPoints.Should().Be(Points.None);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedException_WhenIssuedNotByMentor()
    {
        Submission submission = await Context.GetGithubSubmissionAsync(new ActiveSubmissionState());
        User issuer = await Context.GetNotMentorUser(submission.GroupAssignment, submission.Student.User);
        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestChangesRequested.Command(issuer.Id, submission.Id);
        var handler = new PullRequestChangesRequestedHandler(workflowService);

        await Assert.ThrowsAsync<UnauthorizedException>(() => handler.Handle(command, default));
    }
}