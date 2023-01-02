using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Queries;

public static class GetLastPullRequestSubmission
{
    public record Query(Guid UserId, Guid AssignmentId, long PullRequestNumber) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}