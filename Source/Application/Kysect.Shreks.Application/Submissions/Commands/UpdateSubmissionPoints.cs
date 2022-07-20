using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Exceptions;
using Kysect.Shreks.Core.ValueObject;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Submissions.Commands;

public static class UpdateSubmissionPoints
{
    public record Command(Guid SubmissionId, double NewPoints) : IRequest;

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
            
            //TODO: Should we check if points are between Assignment.MinPoints and Assignment.MaxPoints?
            submission.Points = new Points(request.NewPoints);
            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}