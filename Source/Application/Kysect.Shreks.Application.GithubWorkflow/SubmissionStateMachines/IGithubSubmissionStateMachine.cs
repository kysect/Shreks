using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public interface IGithubSubmissionStateMachine
{
    Task ProcessPullRequestReviewApprove(IShreksCommand? command, GithubPullRequestDescriptor prDescriptor);
    Task ProcessPullRequestReviewRequestChanges(IShreksCommand? command);
    Task ProcessPullRequestReviewComment(IShreksCommand? command);
    Task ProcessPullRequestReopen(bool? isMerged, GithubPullRequestDescriptor prDescriptor);
    Task ProcessPullRequestClosed(bool isMerged, GithubPullRequestDescriptor prDescriptor);
}