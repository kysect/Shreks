using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Submissions.Commands.UpdateSubmissionPoints;

namespace Kysect.Shreks.Application.Handlers.Submissions;

internal class UpdateSubmissionPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IPermissionValidator _permissionValidator;
    private readonly IShreksDatabaseContext _context;

    public UpdateSubmissionPointsHandler(IPermissionValidator permissionValidator, IShreksDatabaseContext context)
    {
        _permissionValidator = permissionValidator;
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        await _permissionValidator.EnsureSubmissionMentorAsync(
            request.IssuerId, request.SubmissionId, cancellationToken);

        Submission submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        Fraction? points = request.RatingPercent is null
            ? null
            : Fraction.FromDenormalizedValue(request.RatingPercent.Value);

        Points? extraPoints = request.ExtraPoints;

        submission.UpdatePoints(points, extraPoints);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        SubmissionRateDto dto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

        return new Response(dto);
    }
}