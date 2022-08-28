using Kysect.Shreks.Application.Dto.Github;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class PullRequestAndAssignmentContext : BaseContext
{
    public GithubPullRequestDescriptor PullRequestDescriptor { get; }
    public Guid AssignmentId { get; }

    public PullRequestAndAssignmentContext(IMediator mediator, Guid issuerId, GithubPullRequestDescriptor pullRequestDescriptor, Guid assignmentId, ILogger log)
        : base(mediator, log, issuerId)
    {
        PullRequestDescriptor = pullRequestDescriptor;
        AssignmentId = assignmentId;
    }
}