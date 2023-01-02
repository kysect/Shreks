using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Submissions.Commands;

internal static class BanSubmission
{
    public record Command(Guid IssuerId, Guid SubmissionId) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}