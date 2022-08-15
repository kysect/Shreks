using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.UpdateSubmissionPayload;
namespace Kysect.Shreks.Application.Handlers.Submissions;

public class UpdateSubmissionPayloadHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public UpdateSubmissionPayloadHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.Payload = request.NewPayload;
        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(submission.ToDto());
    }
}