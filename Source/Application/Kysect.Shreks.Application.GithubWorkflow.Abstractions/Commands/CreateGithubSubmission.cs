using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Commands;

public static class CreateGithubSubmission
{
    public record Command(
        Guid UserId,
        Guid AssignmentId,
        GithubPullRequestDescriptor PullRequestDescriptor) : IRequest<Response>;

    public record Response(SubmissionRateDto Submission);
}