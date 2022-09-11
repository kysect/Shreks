using Kysect.Shreks.Application.Dto.Github;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class PullRequestContext : BaseContext
{
    public GithubPullRequestDescriptor PullRequestDescriptor { get; }

    public PullRequestContext(Guid issuerId, GithubPullRequestDescriptor pullRequestDescriptor)
        : base(issuerId)
    {
        PullRequestDescriptor = pullRequestDescriptor;
    }
}