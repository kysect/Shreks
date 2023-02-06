using Kysect.Shreks.Application.Dto.Submissions;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.GithubEvents;

internal static class PullRequestClosed
{
    public record Command(Guid IssuerId, Guid SubmissionId, bool IsMerged) : IRequest<Response>;

    public record Response(SubmissionActionMessageDto Message);
}