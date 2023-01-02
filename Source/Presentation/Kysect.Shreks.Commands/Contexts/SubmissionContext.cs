using MediatR;

namespace Kysect.Shreks.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public Guid SubmissionId { get; set; }

    public SubmissionContext(Guid issuerId, IMediator mediator, Guid submissionId) : base(issuerId, mediator)
    {
        SubmissionId = submissionId;
    }
}