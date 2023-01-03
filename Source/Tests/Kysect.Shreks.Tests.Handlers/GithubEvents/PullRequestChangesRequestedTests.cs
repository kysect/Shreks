using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Contracts.GithubEvents;
using Kysect.Shreks.Application.Handlers.GithubEvents;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Submissions.States;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers.GithubEvents;

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