using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Commands;

public static class CreateOrUpdateGithubSubmission
{
    public record Command(
        Guid StudentId,
        Guid AssignmentId,
        string Payload,
        string Organization,
        string Repository,
        long PrNumber) : IRequest<Response>;

    public record Response(bool IsCreated, SubmissionDto Submission);
}