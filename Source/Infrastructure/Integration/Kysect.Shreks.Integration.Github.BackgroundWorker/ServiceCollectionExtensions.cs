using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Github.BackgroundWorker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubInviteBackgroundService(this IServiceCollection services)
    {
        return services.AddHostedService<GithubInvitingWorker>();
    }
}