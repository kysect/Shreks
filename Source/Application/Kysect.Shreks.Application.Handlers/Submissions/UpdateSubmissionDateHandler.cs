using AutoMapper;
using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.UpdateSubmissionDate;
namespace Kysect.Shreks.Application.Handlers.Submissions;

public class UpdateSubmissionDateHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public UpdateSubmissionDateHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.SubmissionDate = request.NewDate;
        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}