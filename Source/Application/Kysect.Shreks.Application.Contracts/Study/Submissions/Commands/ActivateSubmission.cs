using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Submissions.Commands;

internal static class ActivateSubmission
{
    public record Command(Guid SubmissionId) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}