using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public SubmissionDto Submission { get; }

    public SubmissionContext(IMediator mediator, Guid issuerId, SubmissionDto submission) 
        : base(mediator, issuerId)
    {
        Submission = submission;
    }
}