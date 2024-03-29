using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Submissions.Commands;

internal static class UpdateSubmissionDate
{
    public record Command(Guid IssuerId, Guid SubmissionId, DateOnly Date) : IRequest<Response>;

    public record Response(SubmissionRateDto Submission);
}