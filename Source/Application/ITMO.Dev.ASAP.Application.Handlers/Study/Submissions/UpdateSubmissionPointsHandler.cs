using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Factories;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.ValueObject;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands.UpdateSubmissionPoints;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;

internal class UpdateSubmissionPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IPermissionValidator _permissionValidator;
    private readonly IPublisher _publisher;

    public UpdateSubmissionPointsHandler(
        IPermissionValidator permissionValidator,
        IDatabaseContext context,
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