using Kysect.Shreks.Core.Study;
using MediatR;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public Submission Submission { get; }

    public SubmissionContext(Mediator mediator, Guid issuerId, Submission submission) : base(mediator, issuerId)
    {
        Submission = submission;
    }
}