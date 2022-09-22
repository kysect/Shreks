using Kysect.Shreks.Application.Commands.Processors;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class PayloadAndAssignmentContext : BaseContext
{
    public ICommandSubmissionFactory CommandSubmissionFactory { get; }
    public Guid AssignmentId { get; }
    public string Payload { get; }

    public PayloadAndAssignmentContext(
        Guid issuerId,
        ICommandSubmissionFactory commandSubmissionFactory,
        Guid assignmentId,
        string payload)
        : base(issuerId)
    {
        CommandSubmissionFactory = commandSubmissionFactory;
        AssignmentId = assignmentId;
        Payload = payload;
    }
}