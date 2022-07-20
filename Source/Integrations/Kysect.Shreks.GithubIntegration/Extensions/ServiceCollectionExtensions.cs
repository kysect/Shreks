using GitHubJwt;
using Kysect.Shreks.GithubIntegration.Client;
using Kysect.Shreks.GithubIntegration.CredentialStores;
using Kysect.Shreks.GithubIntegration.Entities;
using Kysect.Shreks.GithubIntegration.Helpers;
using Kysect.Shreks.GithubIntegration.Processors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Octokit.Webhooks;

namespace Kysect.Shreks.GithubIntegration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubServices(this IServiceCollection services, ShreksConfiguration shreksConfiguration)
    {
        shreksConfiguration.Verify();
        var gitHubConf = shreksConfiguration.GithubConfiguration;
        var cacheConf = shreksConfiguration.CacheConfiguration;

        services.AddSingleton<GitHubJwtFactory>(
            new GitHubJwtFactory(
                new FilePrivateKeySource(gitHubConf.PrivateKeySource),
                new GitHubJwtFactoryOptions
                {
                    AppIntegrationId = gitHubConf.AppIntegrationId, // The GitHub App Id
                    ExpirationSeconds = gitHubConf.ExpirationSeconds // 10 minutes is the maximum time allowed
                }));

        services.AddSingleton<IShreksMemoryCache, ShreksMemoryCache>(_
            => new ShreksMemoryCache(new MemoryCacheOptions
            {
                SizeLimit = cacheConf.SizeLimit,
                ExpirationScanFrequency = TimeSpan.FromMinutes(cacheConf.ExpirationMinutes)
            }));

        services.AddSingleton<IInstallationClientFactory>(_ =>
        {
            var githubJwtFactory = _.GetService<GitHubJwtFactory>()!;
            var memoryCache = _.GetService<IShreksMemoryCache>()!;

            var appClient = new GitHubClient(new ProductHeaderValue(gitHubConf.Organization),
                new GithubAppCredentialStore(githubJwtFactory));
            return new InstallationClientFactory(appClient, memoryCache);
        });

        services.AddSingleton<IActionNotifier, ActionNotifier>();

        return services;
    }

    public static IServiceCollection AddWebhookProcessors(this IServiceCollection services)
    {
        services.AddSingleton<WebhookEventProcessor, ShreksWebhookEventProcessor>();

        return services;
    }
}