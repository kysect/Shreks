using ITMO.Dev.ASAP.Application.Dto.Submissions;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.GithubEvents;

internal static class PullRequestClosed
{
    public record Command(Guid IssuerId, Guid SubmissionId, bool IsMerged) : IRequest<Response>;

    public record Response(SubmissionActionMessageDto Message);
}