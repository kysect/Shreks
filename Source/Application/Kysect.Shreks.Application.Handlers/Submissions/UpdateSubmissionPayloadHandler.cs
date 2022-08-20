using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.UpdateSubmissionPayload;
namespace Kysect.Shreks.Application.Handlers.Submissions;

public class UpdateSubmissionPayloadHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public UpdateSubmissionPayloadHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.Payload = request.NewPayload;
        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}