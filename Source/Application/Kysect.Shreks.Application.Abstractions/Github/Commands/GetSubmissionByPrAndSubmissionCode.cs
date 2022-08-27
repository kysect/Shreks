using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Commands;

public class GetSubmissionByPrAndSubmissionCode
{
    public record Query(GithubPullRequestDescriptor PullRequestDescriptor, int SubmissionCode) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}