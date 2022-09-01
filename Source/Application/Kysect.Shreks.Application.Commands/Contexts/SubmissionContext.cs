using Kysect.Shreks.Application.Dto.Study;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public SubmissionDto Submission { get; }

    public SubmissionContext(IMediator mediator, ILogger log, Guid issuerId, SubmissionDto submission) 
        : base(mediator, log, issuerId)
    {
        Submission = submission;
    }
}