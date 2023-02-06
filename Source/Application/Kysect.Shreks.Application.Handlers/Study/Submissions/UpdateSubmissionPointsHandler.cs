using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Contracts.Study.Submissions.Notifications;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Submissions.Commands.UpdateSubmissionPoints;

namespace Kysect.Shreks.Application.Handlers.Study.Submissions;

internal class UpdateSubmissionPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPermissionValidator _permissionValidator;
    private readonly IPublisher _publisher;

    public UpdateSubmissionPointsHandler(
        IPermissionValidator permissionValidator,
        IShreksDatabaseContext context,
        IPublisher publisher)
    {
        _permissionValidator = permissionValidator;
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        await _permissionValidator.EnsureSubmissionMentorAsync(
            request.IssuerId,
            request.SubmissionId,
            cancellationToken);

        Submission? submission = await _context.Submissions
            .IncludeSubjectCourse()
            .SingleOrDefaultAsync(x => x.Id.Equals(request.SubmissionId), cancellationToken);

        if (submission is null)
            throw EntityNotFoundException.For<Submission>(request.SubmissionId);

        Fraction? points = request.RatingPercent is null
            ? null
            : Fraction.FromDenormalizedValue(request.RatingPercent.Value);

        Points? extraPoints = request.ExtraPoints;

        submission.UpdatePoints(points, extraPoints);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        SubmissionRateDto dto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

        var notification = new SubmissionPointsUpdated.Notification(submission.ToDto());
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}