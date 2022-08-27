using Kysect.Shreks.Application.Dto.Github;
using MediatR;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class PullRequestAndAssignmentContext : BaseContext
{
    public GithubPullRequestDescriptor PullRequestDescriptor { get; }
    public Guid AssignmentId { get; }

    public PullRequestAndAssignmentContext(IMediator mediator, Guid issuerId, GithubPullRequestDescriptor pullRequestDescriptor, Guid assignmentId) : base(mediator, issuerId)
    {
        PullRequestDescriptor = pullRequestDescriptor;
        AssignmentId = assignmentId;
    }
}