using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Models;

namespace Kysect.Shreks.Application.GithubWorkflow.Submissions;

public interface IGithubSubmissionFactory
{
    Task<GithubSubmissionCreationResult> CreateOrUpdateGithubSubmission(
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken);
}