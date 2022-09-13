using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Dto.Github;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public interface IGithubSubmissionStateMachine
{
    Task ProcessPullRequestReviewApprove(IShreksCommand? command, GithubPullRequestDescriptor prDescriptor);
    Task ProcessPullRequestReviewRequestChanges(IShreksCommand? command);
    Task ProcessPullRequestReviewComment(IShreksCommand? command);
    Task ProcessPullRequestReopen(bool? isMerged, GithubPullRequestDescriptor prDescriptor);
    Task ProcessPullRequestClosed(bool merged, GithubPullRequestDescriptor prDescriptor);
}