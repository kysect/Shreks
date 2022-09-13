using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public interface IGithubSubmissionStateMachine
{
    Task ProcessPullRequestReviewApprove(
        IShreksCommand? command,
        ShreksCommandProcessor commandProcessor,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier);

    Task ProcessPullRequestReviewRequestChanges(
        IShreksCommand? command,
        ShreksCommandProcessor commandProcessor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier);

    Task ProcessPullRequestReviewComment(
        IShreksCommand? command,
        ShreksCommandProcessor commandProcessor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier);

    Task ProcessPullRequestReopen(
        bool? isMerged,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommitEventNotifier eventNotifier);

    Task ProcessPullRequestClosed(
        bool merged,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommitEventNotifier eventNotifier);
}