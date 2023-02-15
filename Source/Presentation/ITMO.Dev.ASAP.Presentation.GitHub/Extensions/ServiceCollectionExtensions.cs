using ITMO.Dev.ASAP.Commands.Extensions;
using ITMO.Dev.ASAP.Presentation.GitHub.Notifiers;
using ITMO.Dev.ASAP.Presentation.GitHub.Processing;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace ITMO.Dev.ASAP.Presentation.GitHub.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubPresentation(this IServiceCollection collection)
    {
        collection.AddScoped<WebhookEventProcessor, AsapWebhookEventProcessor>();
        collection.AddScoped<IAsapWebhookEventHandler, AsapWebhookEventHandler>();
        collection.AddScoped<IActionNotifier, ActionNotifier>();
        collection.AddPresentationCommands();

        return collection;
    }
}