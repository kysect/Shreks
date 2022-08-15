using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.CreateSubmissionCommand;
namespace Kysect.Shreks.Application.Handlers.Submissions;


public class CreateSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public CreateSubmissionHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var student = await _context.Students.GetByIdAsync(request.StudentId, cancellationToken);
        var assignment = await _context.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);

        var submission = new Submission(student, assignment, DateTime.Now, request.Payload);
        await _context.Submissions.AddAsync(submission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(submission.ToDto());
    }
}