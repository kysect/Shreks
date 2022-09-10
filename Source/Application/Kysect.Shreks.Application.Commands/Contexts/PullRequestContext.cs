using Kysect.Shreks.Application.Dto.Github;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class PullRequestContext : BaseContext
{
    public GithubPullRequestDescriptor PullRequestDescriptor { get; }

    public PullRequestContext(ILogger log, Guid issuerId, GithubPullRequestDescriptor pullRequestDescriptor)
        : base(log, issuerId)
    {
        PullRequestDescriptor = pullRequestDescriptor;
    }
}