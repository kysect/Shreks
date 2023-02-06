using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Submissions.Commands;

internal static class UpdateSubmissionPoints
{
    public record Command(
        Guid IssuerId,
        Guid SubmissionId,
        double? RatingPercent,
        double? ExtraPoints) : IRequest<Response>;

    public record Response(SubmissionRateDto Submission);
}