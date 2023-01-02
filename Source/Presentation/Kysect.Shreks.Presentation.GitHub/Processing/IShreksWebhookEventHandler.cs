using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Presentation.GitHub.Processing;

public interface IShreksWebhookEventHandler
{
    Task ProcessPullRequestReopen(
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommitEventNotifier eventNotifier,
        CancellationToken cancellationToken);

    Task ProcessPullRequestUpdate(
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommitEventNotifier eventNotifier,
        CancellationToken cancellationToken);

    Task ProcessPullRequestClosed(
        bool merged,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommitEventNotifier eventNotifier,
        CancellationToken cancellationToken);

    Task ProcessPullRequestReviewComment(
        string? comment,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier,
        CancellationToken cancellationToken);

    Task ProcessPullRequestReviewRequestChanges(
        string? reviewBody,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier,
        CancellationToken cancellationToken);

    Task ProcessPullRequestReviewApprove(
        string? commentBody,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier,
        CancellationToken cancellationToken);

    Task ProcessIssueCommentCreate(
        string issueCommentBody,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommentEventNotifier eventNotifier,
        CancellationToken cancellationToken);
}