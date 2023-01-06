using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Queries;

internal static class GetCurrentUnratedSubmission
{
    public record Query(
        string OrganizationName,
        string RepositoryName,
        long PullRequestNumber,
        string Payload) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}