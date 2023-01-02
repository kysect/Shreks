using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Contracts.GithubEvents;
using Kysect.Shreks.Application.Handlers.GithubEvents;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Submissions.States;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers.GithubEvents;

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