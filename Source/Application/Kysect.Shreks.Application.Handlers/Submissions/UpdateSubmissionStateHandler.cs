using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;

using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.UpdateSubmissionState;

namespace Kysect.Shreks.Application.Handlers.Submissions;

public class UpdateSubmissionStateHandler : IRequestHandler<Command, Response>
{
    private readonly IMapper _mapper;
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public UpdateSubmissionStateHandler(IShreksDatabaseContext context, IMapper mapper, ITableUpdateQueue tableUpdateQueue)
    {
        _context = context;
        _mapper = mapper;
        _tableUpdateQueue = tableUpdateQueue;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        PermissionValidator.EnsureHasAccessToRepository(request.UserId, submission);

        submission.State = _mapper.Map<SubmissionState>(request.State);
        await _context.SaveChangesAsync(cancellationToken);

        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());

        var submissionDto = _mapper.Map<SubmissionDto>(submission);
        return new Response(submissionDto);
    }
}