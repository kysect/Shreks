using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Commands;

public static class SyncGithubAdminWithMentors
{
    public record Command(string OrganizationName) : IRequest<Unit>;
}