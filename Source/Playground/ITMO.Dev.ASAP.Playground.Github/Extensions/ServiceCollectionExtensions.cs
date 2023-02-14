using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Google.Extensions;
using ITMO.Dev.ASAP.Application.Handlers.Extensions;
using ITMO.Dev.ASAP.Presentation.GitHub.Extensions;
using Serilog;

namespace ITMO.Dev.ASAP.Playground.Github.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddPlaygroundDependencies(this IServiceCollection servicesCollection)
    {
        return servicesCollection.AddApplicationConfiguration()
            .AddHandlers()
            .AddGithubPresentation()
            .AddDummyGoogleIntegration()
            .AddLogging(logBuilder => logBuilder.AddSerilog());
    }
}