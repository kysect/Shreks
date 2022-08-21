using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Commands;

public static class SyncGithubAdminWithMentors
{
    public record Command(string OrganizationName) : IRequest<Unit>;
}