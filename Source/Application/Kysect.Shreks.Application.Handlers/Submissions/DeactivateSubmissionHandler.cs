using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Submissions.Commands.DeactivateSubmission;

namespace Kysect.Shreks.Application.Handlers.Submissions;

internal class DeactivateSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public DeactivateSubmissionHandler(
        IShreksDatabaseContext context,
        IMapper mapper,
        ITableUpdateQueue tableUpdateQueue)
    {
        _context = context;
        _mapper = mapper;
        _tableUpdateQueue = tableUpdateQueue;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Submission submission = await _context.Submissions
            .IncludeSubjectCourse()
            .IncludeStudentGroup()
            .GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.Deactivate();

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetSubjectCourseId(), submission.GetGroupId());

        SubmissionDto dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}