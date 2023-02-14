using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;

internal static class MarkSubmissionReviewed
{
    public record Command(Guid IssuerId, Guid SubmissionId) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}