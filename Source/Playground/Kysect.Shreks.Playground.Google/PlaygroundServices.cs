using Google.Apis.Auth.OAuth2;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Google.Extensions;
using Kysect.Shreks.Application.Google.Workers;
using Kysect.Shreks.Application.Handlers.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Kysect.Shreks.Playground.Google;

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