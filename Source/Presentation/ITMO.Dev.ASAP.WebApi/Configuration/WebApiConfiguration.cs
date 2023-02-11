using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using ITMO.Dev.ASAP.DataAccess.Configuration;
using ITMO.Dev.ASAP.Integration.Github.Helpers;
using ITMO.Dev.ASAP.Integration.Google.Models;

namespace ITMO.Dev.ASAP.WebApi.Configuration;

internal class WebApiConfiguration
{
    public WebApiConfiguration(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        GoogleIntegrationConfiguration = configuration
            .GetSection(nameof(GoogleIntegrationConfiguration))
            .Get<GoogleIntegrationConfiguration>();
        CacheConfiguration = configuration.GetSection(nameof(CacheConfiguration)).Get<CacheConfiguration>();
        GithubIntegrationConfiguration = configuration
            .GetSection(nameof(GithubIntegrationConfiguration))
            .Get<GithubIntegrationConfiguration>();
        TestEnvironmentConfiguration = configuration
            .GetSection(nameof(TestEnvironmentConfiguration))
            .Get<TestEnvironmentConfiguration>();
        PostgresConfiguration = configuration.GetSection(nameof(PostgresConfiguration)).Get<PostgresConfiguration>();
        DbNamesConfiguration = configuration.GetSection(nameof(DbNamesConfiguration)).Get<DbNamesConfiguration>();
    }

    public GoogleIntegrationConfiguration GoogleIntegrationConfiguration { get; }

    public CacheConfiguration CacheConfiguration { get; }

    public GithubIntegrationConfiguration GithubIntegrationConfiguration { get; }

    public TestEnvironmentConfiguration? TestEnvironmentConfiguration { get; }

    public PostgresConfiguration PostgresConfiguration { get; }

    public DbNamesConfiguration DbNamesConfiguration { get; }
}