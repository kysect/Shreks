using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Dto.Github;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class PullRequestAndAssignmentContext : BaseContext
{
    public GithubPullRequestDescriptor PullRequestDescriptor { get; }
    public Guid AssignmentId { get; }
    public ICommandSubmissionFactory CommandSubmissionFactory { get; }

    public PullRequestAndAssignmentContext(ICommandSubmissionFactory commandSubmissionFactory,
        GithubPullRequestDescriptor pullRequestDescriptor,
        Guid issuerId,
        Guid assignmentId)
        : base(issuerId)
    {
        PullRequestDescriptor = pullRequestDescriptor;
        AssignmentId = assignmentId;
        CommandSubmissionFactory = commandSubmissionFactory;
    }
}