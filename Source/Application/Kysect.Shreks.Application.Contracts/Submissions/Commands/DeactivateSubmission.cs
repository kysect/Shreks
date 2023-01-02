using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Submissions.Commands;

internal static class DeactivateSubmission
{
    public record Command(Guid SubmissionId) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}