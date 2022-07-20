using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Submissions.Commands;

public static class UpdateSubmissionDate
{
    public record Command(Guid SubmissionId, DateTime NewDate) : IRequest;

    public class CommandHandler : IRequestHandler<Command>
    {
        private readonly IShreksDatabaseContext _context;

        public CommandHandler(IShreksDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var submission = await _context.Submissions.FirstOrDefaultAsync(
                s => s.Id == request.SubmissionId,
                cancellationToken
            );
            
            if (submission is null)
                throw new EntityNotFoundException($"Submission with id {request.SubmissionId} not found");
            
            submission.SubmissionDateTime = request.NewDate;
            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}