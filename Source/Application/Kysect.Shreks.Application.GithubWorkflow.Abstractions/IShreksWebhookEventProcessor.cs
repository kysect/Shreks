using Kysect.Shreks.Application.Dto.Github;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public interface IShreksWebhookEventProcessor
{
    Task ProcessPullRequestReopen(bool? isMerged, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestCommitEventNotifier eventNotifier);
    Task ProcessPullRequestUpdate(GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestCommitEventNotifier eventNotifier, CancellationToken cancellationToken);
    Task ProcessPullRequestClosed(bool merged, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestCommitEventNotifier eventNotifier);
    Task ProcessPullRequestReviewComment(string? comment, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestEventNotifier eventNotifier);
    Task ProcessPullRequestReviewRequestChanges(string? reviewBody, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestEventNotifier eventNotifier);
    Task ProcessPullRequestReviewApprove(string? commentBody, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestEventNotifier eventNotifier);
    Task ProcessIssueCommentCreate(string issueCommentBody, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestCommentEventNotifier eventNotifier);
}