using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Commands;

public static class UpdateSubmissionPoints
{
    public record Command(Guid SubmissionId, Guid UserId, double? NewRating, double? ExtraPoints) : IRequest<Response>;

    public record Response(SubmissionRateDto SubmissionRate);
}