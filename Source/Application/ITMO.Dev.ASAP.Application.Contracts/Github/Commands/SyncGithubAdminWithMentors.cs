using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal static class SyncGithubAdminWithMentors
{
    public record Command(string OrganizationName) : IRequest<Unit>;
}