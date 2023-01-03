using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.DataAccess.Configuration;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Google.Models;

namespace Kysect.Shreks.WebApi.Configuration;

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