using ITMO.Dev.ASAP.Application.Abstractions.Submissions.Models;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.GithubEvents;

internal static class PullRequestUpdated
{
    public record Command(
        Guid IssuerId,
        Guid UserId,
        Guid AssignmentId,
        string OrganizationName,
        string RepositoryName,
        long PullRequestNumber,
        string Payload) : IRequest<Response>;

    public record Response(SubmissionUpdateResult Message);
}