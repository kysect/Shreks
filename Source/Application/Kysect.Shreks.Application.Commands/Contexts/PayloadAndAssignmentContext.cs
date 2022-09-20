using Kysect.Shreks.Application.Commands.Processors;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class PayloadAndAssignmentContext : BaseContext
{
    public string Payload { get; }
    public Guid AssignmentId { get; }
    public ICommandSubmissionFactory CommandSubmissionFactory { get; }

    public PayloadAndAssignmentContext(
        ICommandSubmissionFactory commandSubmissionFactory,
        string payload,
        Guid issuerId,
        Guid assignmentId)
        : base(issuerId)
    {
        Payload = payload;
        AssignmentId = assignmentId;
        CommandSubmissionFactory = commandSubmissionFactory;
    }
}