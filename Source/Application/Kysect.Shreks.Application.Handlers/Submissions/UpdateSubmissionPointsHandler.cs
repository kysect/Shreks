using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.UpdateSubmissionPoints;

namespace Kysect.Shreks.Application.Handlers.Submissions;

public class UpdateSubmissionPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;
    private readonly IMapper _mapper;

    public UpdateSubmissionPointsHandler(IShreksDatabaseContext context, ITableUpdateQueue tableUpdateQueue, IMapper mapper)
    {
        _context = context;
        _tableUpdateQueue = tableUpdateQueue;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        if (request.NewRating is null && request.ExtraPoints is null)
        {
            const string ratingName = nameof(request.NewRating);
            const string extraPointsName = nameof(request.ExtraPoints);
            const string message = $"Cannot update submission points, both {ratingName} and {extraPointsName} are null.";
            throw new DomainInvalidOperationException(message);
        }

        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        if (request.NewRating is not null)
        {
            var fraction = new Fraction(request.NewRating.Value / 100);
            submission.Rating = fraction;
        } 

        if (request.ExtraPoints is not null)
            submission.ExtraPoints = new Points(request.ExtraPoints.Value);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _tableUpdateQueue.EnqueueCoursePointsUpdate(submission.GetCourseId());
        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}