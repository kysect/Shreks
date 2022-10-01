using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public interface IGithubSubmissionStateMachine
{
    Task ProcessPullRequestReviewApprove(IShreksCommand? command, GithubPullRequestDescriptor prDescriptor, User sender);
    Task ProcessPullRequestReviewRequestChanges(IShreksCommand? command);
    Task ProcessPullRequestReviewComment(IShreksCommand? command);
    Task ProcessPullRequestReopen(bool? isMerged, GithubPullRequestDescriptor prDescriptor, User sender);
    Task ProcessPullRequestClosed(bool isMerged, GithubPullRequestDescriptor prDescriptor, User sender);
}