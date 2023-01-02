using GitHubJwt;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Client;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Providers;
using Kysect.Shreks.Integration.Github.Client;
using Kysect.Shreks.Integration.Github.CredentialStores;
using Kysect.Shreks.Integration.Github.Entities;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Github.Invites;
using Kysect.Shreks.Integration.Github.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using System.Security.Claims;

namespace Kysect.Shreks.Integration.Github.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubServices(
        this IServiceCollection services,
        CacheConfiguration cacheConfiguration,
        GithubIntegrationConfiguration githubIntegrationConfiguration)
    {
        services.AddClientFactory(cacheConfiguration, githubIntegrationConfiguration);
        services.AddGithubServices();
        services.AddSingleton<IOrganizationDetailsProvider, OrganizationDetailsProvider>();

        return services;
    }

    private static IServiceCollection AddClientFactory(
        this IServiceCollection services,
        CacheConfiguration cacheConfiguration,
        GithubIntegrationConfiguration githubIntegrationConfiguration)
    {
        services.AddSingleton(
            new GitHubJwtFactory(
                new FullStringPrivateKeySource(githubIntegrationConfiguration.GithubAppConfiguration.PrivateKey),
                new GitHubJwtFactoryOptions
                {
                    AppIntegrationId =
                        githubIntegrationConfiguration.GithubAppConfiguration.AppIntegrationId, // The GitHub App Id
                    ExpirationSeconds =
                        githubIntegrationConfiguration.GithubAppConfiguration
                            .JwtExpirationSeconds // 10 minutes is the maximum time allowed
                }));

        services.AddSingleton<IShreksMemoryCache, ShreksMemoryCache>(_ => new ShreksMemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = cacheConfiguration.SizeLimit,
                ExpirationScanFrequency = cacheConfiguration.Expiration
            },
            new MemoryCacheEntryOptions()
                .SetSize(cacheConfiguration.CacheEntryConfiguration.EntrySize)
                .SetAbsoluteExpiration(cacheConfiguration.CacheEntryConfiguration.AbsoluteExpiration)
                .SetSlidingExpiration(cacheConfiguration.CacheEntryConfiguration.SlidingExpiration)
        ));

        services.AddSingleton<IGitHubClient>(serviceProvider =>
        {
            GitHubJwtFactory githubJwtFactory = serviceProvider.GetService<GitHubJwtFactory>()!;

            var appClient = new GitHubClient(new ProductHeaderValue("Kysect.Shreks"),
                new GithubAppCredentialStore(githubJwtFactory));
            return appClient;
        });

        services.AddSingleton<IInstallationClientFactory>(serviceProvider =>
        {
            IGitHubClient appClient = serviceProvider.GetService<IGitHubClient>()!;

            IShreksMemoryCache memoryCache = serviceProvider.GetService<IShreksMemoryCache>()!;

            return new InstallationClientFactory(appClient, memoryCache);
        });

        services.AddSingleton<IOrganizationGithubClientProvider, OrganizationGithubClientProvider>();

        services.AddSingleton<IServiceOrganizationGithubClientProvider, ServiceOrganizationGithubClientProvider>(
            serviceProvider =>
            {
                IGitHubClient appClient = serviceProvider.GetRequiredService<IGitHubClient>();
                IInstallationClientFactory installationClientFactory =
                    serviceProvider.GetRequiredService<IInstallationClientFactory>();

                return new ServiceOrganizationGithubClientProvider(appClient, installationClientFactory,
                    githubIntegrationConfiguration.GithubAppConfiguration.ServiceOrganizationName);
            });

        return services;
    }

    private static IServiceCollection AddGithubAuth(
        this IServiceCollection services,
        GithubAuthConfiguration githubAuthConfiguration)
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
                        context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));

                    return Task.CompletedTask;
                };

                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            });

        return services;
    }

    private static IServiceCollection AddGithubServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ISubjectCourseGithubOrganizationInviteSender, SubjectCourseGithubOrganizationInviteSender>()
            .AddScoped<ISubjectCourseGithubOrganizationRepositoryManager,
                SubjectCourseGithubOrganizationRepositoryManager>()
            .AddScoped<IGithubUserProvider, GithubUserProvider>();
    }
}