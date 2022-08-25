using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Commands;

public static class CreateOrUpdateGithubSubmission
{
    public record Command(
        Guid UserId,
        Guid AssignmentId,
        GithubPullRequestDescriptor PullRequestDescriptor) : IRequest<Response>;

    public record Response(bool IsCreated, SubmissionDto Submission);
}