using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Contracts.GithubEvents;
using Kysect.Shreks.Application.Handlers.GithubEvents;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Submissions.States;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers.GithubEvents;

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
        User issuer = await Context.GetNotMentorUser(submission.GroupAssignment);

        var command = new PullRequestApproved.Command(issuer.Id, submission.Id);
        var handler = new PullRequestApprovedHandler(workflowService);

        Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedException>();
    }
}