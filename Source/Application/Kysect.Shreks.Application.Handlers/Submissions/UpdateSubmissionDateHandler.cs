using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.ValueObject;
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

        DeadlinePolicy? deadlinePolicy = submission.GetActiveDeadlinePolicy(submission.GroupAssignment.Deadline);

        if (deadlinePolicy is null)
            throw new DomainInvalidOperationException($"Failed to find deadline policy for group assignment {submission.GroupAssignment}");

        Points deadlineAppliedPoints = deadlinePolicy.Apply(submission.Points!.Value);

        var dto = new SubmissionRateDto(
            Code: submission.Code,
            SubmissionDate: submission.SubmissionDate,
            Rating: submission.Rating?.Value,
            RawPoints: submission.Points?.Value,
            ExtraPoints: submission.ExtraPoints?.Value,
            PenaltyPoints: (submission.Points - deadlineAppliedPoints)?.Value,
            TotalPoints: (deadlineAppliedPoints + submission.ExtraPoints)?.Value
        );

        return new Response(dto);
    }
}