using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Commands;

public static class UpdateSubmissionState
{
    public record struct Command(Guid UserId, Guid SubmissionId, SubmissionStateDto State) : IRequest<Response>;

    public record struct Response(SubmissionDto Submission);
}