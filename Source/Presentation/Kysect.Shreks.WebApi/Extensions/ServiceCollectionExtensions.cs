using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Controllers;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Identity.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Presentation.GitHub.Extensions;
using Kysect.Shreks.WebApi.Configuration;
using Kysect.Shreks.WebApi.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kysect.Shreks.WebApi.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection ConfigureServiceCollection(this IServiceCollection serviceCollection,
        WebApiConfiguration webApiConfiguration,
        IConfigurationSection identityConfigurationSection,
        bool isDevelopmentEnvironment)
    {
        if (webApiConfiguration.TestEnvironmentConfiguration is not null)
            serviceCollection.TryAddSingleton(webApiConfiguration.TestEnvironmentConfiguration);

        serviceCollection
            .AddControllers(x => x.Filters.Add<AuthenticationFilter>())
            .AddNewtonsoftJson()
            .AddApplicationPart(typeof(IControllersProjectMarker).Assembly)
            .AddControllersAsServices();

        serviceCollection
            .AddSwagger()
            .AddApplicationConfiguration()
            .AddHandlers()
            .AddGithubPresentation()
            .AddMappingConfiguration()
            .AddDatabaseContext(o => o
                .UseNpgsql(webApiConfiguration.PostgresConfiguration.ToConnectionString(webApiConfiguration
                    .DbNamesConfiguration.ApplicationDbName))
                .UseLazyLoadingProxies());

        serviceCollection.AddIdentityConfiguration(
            identityConfigurationSection,
            x => x.UseNpgsql(
                webApiConfiguration.PostgresConfiguration.ToConnectionString(webApiConfiguration.DbNamesConfiguration
                    .IdentityDbName)));

        serviceCollection
            .AddGoogleIntegrationServices(webApiConfiguration)
            .AddGithubServices(webApiConfiguration.CacheConfiguration, webApiConfiguration.GithubIntegrationConfiguration)
            .AddGithubWorkflowServices();

        if (isDevelopmentEnvironment && webApiConfiguration.TestEnvironmentConfiguration is not null)
        {
            serviceCollection.AddEntityGeneratorsAndSeeding(webApiConfiguration.TestEnvironmentConfiguration);
        }

        serviceCollection.AddRazorPages();

        return serviceCollection;
    }
}