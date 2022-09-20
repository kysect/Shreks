using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.GithubWorkflow.Submissions;

public interface IGithubSubmissionFactory
{
    Task<GithubSubmissionCreationResult> CreateOrUpdateGithubSubmission(
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken);

    Task<GithubSubmission> CreateGithubSubmissionAsync(
        Guid userId,
        Guid assignmentId,
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken);
}