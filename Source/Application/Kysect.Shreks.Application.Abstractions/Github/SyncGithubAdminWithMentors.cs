using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github;

public static class SyncGithubAdminWithMentors
{
    public record Command(string OrganizationName) : IRequest<Unit>;
}