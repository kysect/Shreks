using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Common.Exceptions;
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

    public UpdateSubmissionPointsHandler(IShreksDatabaseContext context, ITableUpdateQueue tableUpdateQueue)
    {
        _context = context;
        _tableUpdateQueue = tableUpdateQueue;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        if (!PermissionValidator.IsRepositoryMentor(request.UserId, submission))
        {
            throw new UnauthorizedException("Only mentors can rate submission");
        }

        Fraction? fraction = request.NewRating is null ? null : new Fraction(request.NewRating.Value / 100);
        Points? extraPoints = request.ExtraPoints is null ? null : new Points(request.ExtraPoints.Value);

        submission.Rate(fraction, extraPoints);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _tableUpdateQueue.EnqueueCoursePointsUpdate(submission.GetCourseId());
        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());

        SubmissionRateDto dto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

        return new Response(dto);
    }
}