using MediatR;

namespace Kysect.Shreks.Commands.Contexts;

public class PayloadAndAssignmentContext : BaseContext
{
    public PayloadAndAssignmentContext(Guid issuerId, IMediator mediator, Guid assignmentId, string payload)
        : base(issuerId, mediator)
    {
        AssignmentId = assignmentId;
        Payload = payload;
    }

    public Guid AssignmentId { get; }
    public string Payload { get; }
}