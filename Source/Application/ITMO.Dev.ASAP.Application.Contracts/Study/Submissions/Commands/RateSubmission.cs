using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;

internal static class RateSubmission
{
    public record Command(
        Guid IssuerId,
        Guid SubmissionId,
        double? RatingPercent,
        double? ExtraPoints) : IRequest<Response>;

    public record Response(SubmissionRateDto Submission);
}