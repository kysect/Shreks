using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Queries;

internal static class GetLastPullRequestSubmission
{
    public record Query(Guid UserId, Guid AssignmentId, long PullRequestNumber) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}