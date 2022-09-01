using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Factory;

public interface ISubmissionFactory
{
    Task<GithubSubmission> CreateGithubSubmissionAsync(
        Guid userId,
        Guid assignmentId,
        GithubPullRequestDescriptor pullRequestDescriptor,
        CancellationToken cancellationToken);
}