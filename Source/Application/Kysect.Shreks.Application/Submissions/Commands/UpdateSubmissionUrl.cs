using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Submissions.Commands;

public static class UpdateSubmissionUrl
{
    public record Command(Guid SubmissionId, string NewUrl) : IRequest;

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

            // TODO: Create submission with submissionUrl
            throw new NotImplementedException();
        }
    }
}