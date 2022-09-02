using System.Security.Claims;
using GitHubJwt;
using Kysect.Shreks.Application.Abstractions.Github;
using Kysect.Shreks.Integration.Github.Client;
using Kysect.Shreks.Integration.Github.CredentialStores;
using Kysect.Shreks.Integration.Github.Entities;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Github.Invites;
using Kysect.Shreks.Integration.Github.Processors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubServices(
        this IServiceCollection services,
        CacheConfiguration cacheConfiguration,
        GithubIntegrationConfiguration githubIntegrationConfiguration)
    {
        services.AddClientFactory(cacheConfiguration, githubIntegrationConfiguration);
        services.AddGithubAuth(githubIntegrationConfiguration.AuthConfiguration);
        services.AddScoped<IActionNotifier, ActionNotifier>();
        services.AddScoped<WebhookEventProcessor, ShreksWebhookEventProcessorProxy>();
        services.AddGithubInviteBackgroundService();
        services.AddSingleton<IOrganizationDetailsProvider, OrganizationDetailsProvider>();

        return services;
    }

    private static IServiceCollection AddClientFactory(
        this IServiceCollection services,
        CacheConfiguration cacheConfiguration,
        GithubIntegrationConfiguration githubIntegrationConfiguration)
    {
        services.AddSingleton<GitHubJwtFactory>(
            new GitHubJwtFactory(
                new FilePrivateKeySource(githubIntegrationConfiguration.GithubAppConfiguration.PrivateKeySource),
                new GitHubJwtFactoryOptions
                {
                    AppIntegrationId = githubIntegrationConfiguration.GithubAppConfiguration.AppIntegrationId, // The GitHub App Id
                    ExpirationSeconds = githubIntegrationConfiguration.GithubAppConfiguration.JwtExpirationSeconds // 10 minutes is the maximum time allowed
                }));

        services.AddSingleton<IShreksMemoryCache, ShreksMemoryCache>(_ => new ShreksMemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = cacheConfiguration.SizeLimit,
                ExpirationScanFrequency = cacheConfiguration.Expiration,
            },
            new MemoryCacheEntryOptions()
                .SetSize(cacheConfiguration.CacheEntryConfiguration.EntrySize)
                .SetAbsoluteExpiration(cacheConfiguration.CacheEntryConfiguration.AbsoluteExpiration)
                .SetSlidingExpiration(cacheConfiguration.CacheEntryConfiguration.SlidingExpiration)
        ));

        services.AddSingleton<IGitHubClient>(serviceProvider =>
        {
            var githubJwtFactory = serviceProvider.GetService<GitHubJwtFactory>()!;

            var appClient = new GitHubClient(new ProductHeaderValue("Kysect.Shreks"),
                new GithubAppCredentialStore(githubJwtFactory));
            return appClient;
        });

        services.AddSingleton<IInstallationClientFactory>(serviceProvider =>
        {
            var appClient = serviceProvider.GetService<IGitHubClient>()!;

            var memoryCache = serviceProvider.GetService<IShreksMemoryCache>()!;

            return new InstallationClientFactory(appClient, memoryCache);
        });

        services.AddSingleton<IOrganizationGithubClientProvider, OrganizationGithubClientProvider>();

        return services;
    }

    private static IServiceCollection AddGithubAuth(this IServiceCollection services, GithubAuthConfiguration githubAuthConfiguration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/api/auth/github/sign-in";
        })
        .AddGitHub(options =>
        {
            options.ClientId = githubAuthConfiguration.OAuthClientId!;
            options.ClientSecret = githubAuthConfiguration.OAuthClientSecret!;

            options.CallbackPath = new PathString("/signin-github");
            options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
            options.TokenEndpoint = "https://github.com/login/oauth/access_token";
            options.UserInformationEndpoint = "https://api.github.com/user";

            options.Scope.Add("read:user");

            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
            options.ClaimActions.MapJsonKey("urn:github:name", "name");
            options.ClaimActions.MapJsonKey("urn:github:url", "url");

            options.Events.OnCreatingTicket += context =>
            {
                if (context.AccessToken is not null)
                {
                    context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
                }

                return Task.CompletedTask;
            };

            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        });

        return services;
    }

    private static IServiceCollection AddGithubInviteBackgroundService(this IServiceCollection services)
    {
        return services.AddHostedService<GithubInvitingWorker>();
    }
}