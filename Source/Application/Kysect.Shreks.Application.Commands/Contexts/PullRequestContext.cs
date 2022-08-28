using Kysect.Shreks.Application.Dto.Github;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class PullRequestContext : BaseContext
{
    public GithubPullRequestDescriptor PullRequestDescriptor { get; }

    public PullRequestContext(IMediator mediator, ILogger log, Guid issuerId, GithubPullRequestDescriptor pullRequestDescriptor)
        : base(mediator, log, issuerId)
    {
        PullRequestDescriptor = pullRequestDescriptor;
    }
}