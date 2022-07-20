using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Exceptions;
using Kysect.Shreks.Core.Study;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Submissions.Commands;

public static class CreateSubmissionCommand
{
    public record Command(Guid StudentId, Guid AssignmentId, string SubmissionUrl) : IRequest<Response>;

    public record Response(Guid SubmissionId);

    public class CommandHandler : IRequestHandler<Command, Response>
    {
        private readonly IShreksDatabaseContext _context;

        public CommandHandler(IShreksDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var student = await _context.Students.FirstOrDefaultAsync(
                                            s => s.Id == request.StudentId,
                                            cancellationToken
                                        );

            if (student is null)
                throw new EntityNotFoundException($"Student with id {request.StudentId} not found");

            var assignment = await _context.Assignments.FirstOrDefaultAsync(
                                            a => a.Id == request.AssignmentId,
                                            cancellationToken
                                        );

            if (assignment is null)
                throw new EntityNotFoundException($"Assignment with id {request.AssignmentId} not found");

            var studentAssignment = await _context.StudentAssignments.FirstOrDefaultAsync(
                        sa => sa.Student.Equals(student) && sa.Assignment.Equals(assignment),
                        cancellationToken
                );

            if (studentAssignment is null)
            {
                studentAssignment = new StudentAssignment(student, assignment);
                await _context.StudentAssignments.AddAsync(studentAssignment, cancellationToken);
            }

            // TODO: Create submission with submissionUrl
            throw new NotImplementedException();
        }
    }
}