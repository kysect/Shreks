using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;

internal static class UpdateSubmissionDate
{
    public record Command(Guid IssuerId, Guid SubmissionId, DateOnly Date) : IRequest<Response>;

    public record Response(SubmissionRateDto Submission);
}