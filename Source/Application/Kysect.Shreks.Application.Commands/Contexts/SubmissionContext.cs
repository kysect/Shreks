using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using MediatR;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public readonly Submission Submission;

    public SubmissionContext(Mediator mediator, User issuer, Submission submission) : base(mediator, issuer)
    {
        Submission = submission;
    }
}