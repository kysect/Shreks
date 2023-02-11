using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using ITMO.Dev.ASAP.Integration.Github.Extensions;
using ITMO.Dev.ASAP.Integration.Github.Helpers;
using ITMO.Dev.ASAP.Playground.Github.Extensions;
using ITMO.Dev.ASAP.Playground.Github.TestEnv;
using Serilog;

namespace ITMO.Dev.ASAP.Playground.Github;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File("AsapGithubPlayground.log")
            .CreateLogger();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        CacheConfiguration cacheConfiguration =
            builder.Configuration.GetSection(nameof(CacheConfiguration)).Get<CacheConfiguration>();

        GithubIntegrationConfiguration githubIntegrationConfiguration = builder.Configuration
            .GetSection(nameof(GithubIntegrationConfiguration))
            .Get<GithubIntegrationConfiguration>();

        TestEnvironmentConfiguration testEnvironmentConfiguration = builder.Configuration
            .GetSection(nameof(TestEnvironmentConfiguration))
            .Get<TestEnvironmentConfiguration>();

        builder.Services
            .AddPlaygroundDependencies()
            .AddGithubServices(cacheConfiguration, githubIntegrationConfiguration)
            .AddGithubPlaygroundDatabase(testEnvironmentConfiguration);

        WebApplication app = builder.Build();

        app.UseGithubIntegration(githubIntegrationConfiguration);
        await app.Services.UseTestEnv(testEnvironmentConfiguration);

        await app.RunAsync();
    }
}