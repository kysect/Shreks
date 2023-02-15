using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;

internal static class DeactivateSubmission
{
    public record Command(Guid SubmissionId) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}