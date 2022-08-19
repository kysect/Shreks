using GitHubJwt;
using Kysect.Shreks.Integration.Github.Client;
using Kysect.Shreks.Integration.Github.CredentialStores;
using Kysect.Shreks.Integration.Github.Entities;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Github.Processors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubServices(this IServiceCollection services, ShreksConfiguration shreksConfiguration)
    {
        var gitHubConf = shreksConfiguration.GithubConfiguration;
        var cacheConf = shreksConfiguration.CacheConfiguration;
        var cacheEntryConf = shreksConfiguration.CacheEntryConfiguration;

        services.AddSingleton<GitHubJwtFactory>(
            new GitHubJwtFactory(
                new FilePrivateKeySource(gitHubConf.PrivateKeySource),
                new GitHubJwtFactoryOptions
                {
                    AppIntegrationId = gitHubConf.AppIntegrationId, // The GitHub App Id
                    ExpirationSeconds = gitHubConf.ExpirationSeconds // 10 minutes is the maximum time allowed
                }));

        services.AddSingleton<IShreksMemoryCache, ShreksMemoryCache>(_ => new ShreksMemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = cacheConf.SizeLimit,
                ExpirationScanFrequency = cacheConf.Expiration,
            },
            new MemoryCacheEntryOptions()
                .SetSize(cacheEntryConf.EntrySize)
                .SetAbsoluteExpiration(cacheEntryConf.AbsoluteExpiration)
                .SetSlidingExpiration(cacheEntryConf.SlidingExpiration)
            ));

        services.AddSingleton<IGitHubClient>(serviceProvider =>
        {
            var githubJwtFactory = serviceProvider.GetService<GitHubJwtFactory>()!;

            var appClient = new GitHubClient(new ProductHeaderValue(gitHubConf.Organization),
                new GithubAppCredentialStore(githubJwtFactory));
            return appClient;
        });

        services.AddSingleton<IInstallationClientFactory>(serviceProvider =>
        {
            var appClient = serviceProvider.GetService<IGitHubClient>()!;

            var memoryCache = serviceProvider.GetService<IShreksMemoryCache>()!;

            return new InstallationClientFactory(appClient, memoryCache);
        });

        services.AddScoped<IActionNotifier, ActionNotifier>();

        services.AddScoped<WebhookEventProcessor, ShreksWebhookEventProcessor>();
        
        return services;
    }
}