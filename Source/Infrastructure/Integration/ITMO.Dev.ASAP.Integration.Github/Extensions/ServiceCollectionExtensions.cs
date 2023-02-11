using GitHubJwt;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Client;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Providers;
using ITMO.Dev.ASAP.Integration.Github.Client;
using ITMO.Dev.ASAP.Integration.Github.CredentialStores;
using ITMO.Dev.ASAP.Integration.Github.Entities;
using ITMO.Dev.ASAP.Integration.Github.Helpers;
using ITMO.Dev.ASAP.Integration.Github.Invites;
using ITMO.Dev.ASAP.Integration.Github.Providers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace ITMO.Dev.ASAP.Integration.Github.Extensions;

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
        if (string.IsNullOrWhiteSpace(githubIntegrationConfiguration.GithubAppConfiguration.PrivateKey))
        {
            const string message = @"Github app configuration is missing private key";
            throw new ArgumentException(message, nameof(githubIntegrationConfiguration));
        }

        services.AddSingleton(
            new GitHubJwtFactory(
                new FullStringPrivateKeySource(githubIntegrationConfiguration.GithubAppConfiguration.PrivateKey),
                new GitHubJwtFactoryOptions
                {
                    AppIntegrationId =
                        githubIntegrationConfiguration.GithubAppConfiguration.AppIntegrationId, // The GitHub App Id
                    ExpirationSeconds =
                        githubIntegrationConfiguration.GithubAppConfiguration
                            .JwtExpirationSeconds, // 10 minutes is the maximum time allowed
                }));

        services.AddSingleton<IAsapMemoryCache, AsapMemoryCache>(_ => new AsapMemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = cacheConfiguration.SizeLimit, ExpirationScanFrequency = cacheConfiguration.Expiration,
            },
            new MemoryCacheEntryOptions()
                .SetSize(cacheConfiguration.CacheEntryConfiguration.EntrySize)
                .SetAbsoluteExpiration(cacheConfiguration.CacheEntryConfiguration.AbsoluteExpiration)
                .SetSlidingExpiration(cacheConfiguration.CacheEntryConfiguration.SlidingExpiration)));

        services.AddSingleton<IGitHubClient>(serviceProvider =>
        {
            GitHubJwtFactory githubJwtFactory = serviceProvider.GetService<GitHubJwtFactory>()!;

            var appClient = new GitHubClient(
                new ProductHeaderValue("ITMO.Dev.ASAP"),
                new GithubAppCredentialStore(githubJwtFactory));
            return appClient;
        });

        services.AddSingleton<IInstallationClientFactory>(serviceProvider =>
        {
            IGitHubClient appClient = serviceProvider.GetService<IGitHubClient>()!;

            IAsapMemoryCache memoryCache = serviceProvider.GetService<IAsapMemoryCache>()!;

            return new InstallationClientFactory(appClient, memoryCache);
        });

        services.AddSingleton<IOrganizationGithubClientProvider, OrganizationGithubClientProvider>();

        services.AddSingleton<IServiceOrganizationGithubClientProvider, ServiceOrganizationGithubClientProvider>(
            serviceProvider =>
            {
                string? serviceOrganizationName = githubIntegrationConfiguration
                    .GithubAppConfiguration.ServiceOrganizationName;

                if (string.IsNullOrWhiteSpace(serviceOrganizationName))
                {
                    const string message = @"GitHub Service Organization Name is missing";
                    throw new ArgumentException(message, nameof(githubIntegrationConfiguration));
                }

                IGitHubClient appClient = serviceProvider.GetRequiredService<IGitHubClient>();
                IInstallationClientFactory installationClientFactory =
                    serviceProvider.GetRequiredService<IInstallationClientFactory>();

                return new ServiceOrganizationGithubClientProvider(
                    appClient,
                    installationClientFactory,
                    serviceOrganizationName);
            });

        return services;
    }

    /*
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
    */
    private static IServiceCollection AddGithubServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ISubjectCourseGithubOrganizationInviteSender, SubjectCourseGithubOrganizationInviteSender>()
            .AddScoped<ISubjectCourseGithubOrganizationRepositoryManager,
                SubjectCourseGithubOrganizationRepositoryManager>()
            .AddScoped<IGithubUserProvider, GithubUserProvider>();
    }
}