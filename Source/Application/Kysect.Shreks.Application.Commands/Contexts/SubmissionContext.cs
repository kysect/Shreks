using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using MediatR;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public Submission Submission { get; }

    public SubmissionContext(Mediator mediator, CancellationToken cancellationToken, User issuer, Submission submission) 
        : base(mediator, cancellationToken, issuer)
    {
        Submission = submission;
    }
}