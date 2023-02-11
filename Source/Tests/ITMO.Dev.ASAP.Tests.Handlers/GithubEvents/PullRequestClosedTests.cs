using FluentAssertions;
using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Contracts.GithubEvents;
using ITMO.Dev.ASAP.Application.Handlers.GithubEvents;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Submissions.States;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Core.ValueObject;
using ITMO.Dev.ASAP.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.GithubEvents;

public class PullRequestClosedTests : GithubEventsTestBase
{
    [Fact]
    public async Task Handle_ShouldCompleteSubmission_WhenMergedByMentor()
    {
        Submission submission = await Context.GetGithubSubmissionAsync(new ActiveSubmissionState());
        Mentor issuer = submission.GetMentor();
        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();
        IPermissionValidator permissionValidator = Provider.GetRequiredService<IPermissionValidator>();

        var command = new PullRequestClosed.Command(issuer.UserId, submission.Id, true);
        var handler = new PullRequestClosedHandler(workflowService, permissionValidator);

        await handler.Handle(command, CancellationToken.None);

        submission.State.Should().BeOfType<CompletedSubmissionState>();
        submission.Rating.Should().Be(Fraction.FromDenormalizedValue(100));
        submission.ExtraPoints.Should().Be(Points.None);
    }

    [Fact]
    public async Task Handle_ShouldDeactivateSubmission_WhenClosedByMentor()
    {
        Submission submission = await Context.GetGithubSubmissionAsync(new ActiveSubmissionState());
        Mentor issuer = submission.GetMentor();
        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();
        IPermissionValidator permissionValidator = Provider.GetRequiredService<IPermissionValidator>();

        var command = new PullRequestClosed.Command(issuer.UserId, submission.Id, false);
        var handler = new PullRequestClosedHandler(workflowService, permissionValidator);

        await handler.Handle(command, CancellationToken.None);

        submission.State.Should().BeOfType<InactiveSubmissionState>();
    }

    [Fact]
    public async Task Handle_ShouldCompleteSubmission_WhenMergedByAuthor()
    {
        Submission submission = await Context.GetGithubSubmissionAsync(new ActiveSubmissionState());
        Student issuer = submission.Student;
        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();
        IPermissionValidator permissionValidator = Provider.GetRequiredService<IPermissionValidator>();

        var command = new PullRequestClosed.Command(issuer.UserId, submission.Id, true);
        var handler = new PullRequestClosedHandler(workflowService, permissionValidator);

        await handler.Handle(command, CancellationToken.None);

        submission.State.Should().BeOfType<CompletedSubmissionState>();
        submission.Rating.Should().BeNull();
        submission.ExtraPoints.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldDeactivateSubmission_WhenClosedByAuthor()
    {
        Submission submission = await Context.GetGithubSubmissionAsync(new ActiveSubmissionState());
        Student issuer = submission.Student;
        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();
        IPermissionValidator permissionValidator = Provider.GetRequiredService<IPermissionValidator>();

        var command = new PullRequestClosed.Command(issuer.UserId, submission.Id, false);
        var handler = new PullRequestClosedHandler(workflowService, permissionValidator);

        await handler.Handle(command, CancellationToken.None);

        submission.State.Should().BeOfType<InactiveSubmissionState>();
        submission.Rating.Should().BeNull();
        submission.ExtraPoints.Should().BeNull();
    }
}