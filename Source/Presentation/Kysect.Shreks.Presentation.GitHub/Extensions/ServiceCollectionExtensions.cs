using Kysect.Shreks.Commands.Extensions;
using Kysect.Shreks.Presentation.GitHub.Notifiers;
using Kysect.Shreks.Presentation.GitHub.Processing;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace Kysect.Shreks.Presentation.GitHub.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubPresentation(this IServiceCollection collection)
    {
        collection.AddScoped<WebhookEventProcessor, ShreksWebhookEventProcessor>();
        collection.AddScoped<IShreksWebhookEventHandler, ShreksWebhookEventHandler>();
        collection.AddScoped<IActionNotifier, ActionNotifier>();
        collection.AddPresentationCommands();

        return collection;
    }
}