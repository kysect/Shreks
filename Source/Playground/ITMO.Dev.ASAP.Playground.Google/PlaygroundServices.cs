using Google.Apis.Auth.OAuth2;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Google.Extensions;
using ITMO.Dev.ASAP.Application.Google.Workers;
using ITMO.Dev.ASAP.Application.Handlers.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ITMO.Dev.ASAP.Playground.Google;

internal static class PlaygroundServices
{
    internal static async Task<IServiceProvider> BuildServiceProvider()
    {
        GoogleCredential googleCredentials = await GoogleCredential.FromFileAsync("client_secrets.json", default);

        IServiceProvider serviceProvider = new ServiceCollection()
            .AddApplicationConfiguration()
            .AddGoogleIntegration(o => o
                .ConfigureGoogleCredentials(googleCredentials)
                .ConfigureDriveId("17CfXw__b4nnPp7VEEgWGe-N8VptaL1hP"))
            .AddHandlers()
            .AddLogging(o => o.AddSerilog())
            .AddSingleton<GoogleTableUpdateWorker>()
            .AddGooglePlaygroundDatabase()
            .BuildServiceProvider();
        return serviceProvider;
    }
}