using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.GithubWorkflow.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddGithubInviteBackgroundService(this IServiceCollection services)
    {
        return services
            .AddHostedService<GithubInvitingWorker>();
    }
}
