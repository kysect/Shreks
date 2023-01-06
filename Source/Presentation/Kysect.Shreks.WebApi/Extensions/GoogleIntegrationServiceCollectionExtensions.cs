using Google.Apis.Auth.OAuth2;
using Kysect.Shreks.Application.Google.Extensions;
using Kysect.Shreks.WebApi.Configuration;

namespace Kysect.Shreks.WebApi.Extensions;

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