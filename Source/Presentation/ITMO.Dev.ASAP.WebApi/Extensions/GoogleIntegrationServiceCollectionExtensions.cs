using Google.Apis.Auth.OAuth2;
using ITMO.Dev.ASAP.Application.Google.Extensions;
using ITMO.Dev.ASAP.WebApi.Configuration;

namespace ITMO.Dev.ASAP.WebApi.Extensions;

internal static class GoogleIntegrationServiceCollectionExtensions
{
    internal static IServiceCollection AddGoogleIntegrationServices(
        this IServiceCollection serviceCollection,
        WebApiConfiguration webApiConfiguration)
    {
        if (!webApiConfiguration.GoogleIntegrationConfiguration.EnableGoogleIntegration)
            return serviceCollection.AddDummyGoogleIntegration();

        return serviceCollection
            .AddGoogleIntegration(o => o
                .ConfigureGoogleCredentials(
                    GoogleCredential.FromJson(webApiConfiguration.GoogleIntegrationConfiguration.ClientSecrets))
                .ConfigureDriveId(webApiConfiguration.GoogleIntegrationConfiguration.GoogleDriveId));
    }
}