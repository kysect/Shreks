using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal static class SyncGithubMentors
{
    public record Command(string OrganizationName) : IRequest<Unit>;
}