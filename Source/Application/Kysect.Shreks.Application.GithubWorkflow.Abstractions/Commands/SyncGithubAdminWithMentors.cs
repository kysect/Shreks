using MediatR;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Commands;

public static class SyncGithubAdminWithMentors
{
    public record Command(string OrganizationName) : IRequest<Unit>;
}