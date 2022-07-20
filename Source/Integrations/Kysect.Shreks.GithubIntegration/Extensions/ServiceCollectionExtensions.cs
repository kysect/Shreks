using GitHubJwt;
using Kysect.Shreks.GithubIntegration.Client;
using Kysect.Shreks.GithubIntegration.CredentialStores;
using Kysect.Shreks.GithubIntegration.Helpers;
using Kysect.Shreks.GithubIntegration.Processors;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Octokit.Webhooks;

namespace Kysect.Shreks.GithubIntegration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubServices(this IServiceCollection services, GithubConfiguration githubConf)
    {
        services.AddSingleton<GitHubJwtFactory>(
            new GitHubJwtFactory(
                new FilePrivateKeySource(githubConf.PrivateKeySource),
                new GitHubJwtFactoryOptions
                {
                    AppIntegrationId = githubConf.AppIntegrationId, // The GitHub App Id
                    ExpirationSeconds = githubConf.ExpirationSeconds // 10 minutes is the maximum time allowed
                }));

        services.AddSingleton<IInstallationClientFactory>(_ =>
        {
            var githubJwtFactory = _.GetService<GitHubJwtFactory>();

            var appClient =  new GitHubClient(new ProductHeaderValue(githubConf.Organization), new GithubAppCredentialStore(githubJwtFactory));
            return new InstallationClientFactory(appClient);
        });

        return services;
    }

    public static IServiceCollection AddWebhookProcessors(this IServiceCollection services)
    {
        services.AddSingleton<WebhookEventProcessor, CustomWebhookEventProcessor>();

        return services;
    }
}