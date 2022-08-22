using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.UpdateSubmissionDate;
namespace Kysect.Shreks.Application.Handlers.Submissions;

public class UpdateSubmissionDateHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;
    private readonly IMapper _mapper;

    public UpdateSubmissionDateHandler(IShreksDatabaseContext context, ITableUpdateQueue tableUpdateQueue, IMapper mapper)
    {
        _context = context;
        _tableUpdateQueue = tableUpdateQueue;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.SubmissionDate = request.NewDate;
        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}