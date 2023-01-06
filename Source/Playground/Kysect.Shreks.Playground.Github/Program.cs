using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Playground.Github.Extensions;
using Kysect.Shreks.Playground.Github.TestEnv;
using Serilog;

namespace Kysect.Shreks.Playground.Github;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File("ShreksGithubPlayground.log")
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