using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Queries;

public static class GetCurrentUnratedSubmissionByPrNumber
{
    public record Query(GithubPullRequestDescriptor PullRequestDescriptor) : IRequest<Response>;

    public record Response(SubmissionDto SubmissionDto);
}