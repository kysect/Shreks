using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Study.Commands.CompromiseSubmission;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class CompromiseSubmissionHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;

    public CompromiseSubmissionHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        Submission submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.IsCompromised = true;

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}