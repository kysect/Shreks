using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Commands;

public static class ActivateSubmission
{
    public record struct Command(Guid UserId, Guid SubmissionId) : IRequest<Response>;

    public record struct Response(SubmissionDto Submission);
}