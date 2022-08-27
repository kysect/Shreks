using Kysect.Shreks.Application.Dto.Github;
using MediatR;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class PullRequestContext : BaseContext
{
    public GithubPullRequestDescriptor PullRequestDescriptor { get; }

    public PullRequestContext(IMediator mediator, Guid issuerId, GithubPullRequestDescriptor pullRequestDescriptor)
        : base(mediator, issuerId)
    {
        PullRequestDescriptor = pullRequestDescriptor;
    }
}