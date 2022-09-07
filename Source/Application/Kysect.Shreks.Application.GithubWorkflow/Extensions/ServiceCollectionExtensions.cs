using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.GithubWorkflow.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubInviteBackgroundService(this IServiceCollection services)
    {
        return services
            .AddScoped<IShreksWebhookEventProcessor, ShreksWebhookEventProcessor>()
            .AddHostedService<GithubInvitingWorker>();
    }
}
