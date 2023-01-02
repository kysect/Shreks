using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.Google.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Playground.Github.TestEnv;
using Kysect.Shreks.Presentation.GitHub.Extensions;
using Serilog;

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
    .AddApplicationConfiguration()
    .AddMappingConfiguration()
    .AddHandlers()
    .AddGithubPresentation()
    .AddGithubServices(cacheConfiguration, githubIntegrationConfiguration)
    .AddGithubPlaygroundDatabase(testEnvironmentConfiguration)
    .AddDummyGoogleIntegration();

builder.Services
    .AddLogging(logBuilder => logBuilder.AddSerilog());

WebApplication app = builder.Build();

app.UseGithubIntegration(githubIntegrationConfiguration);
await app.Services.UseTestEnv(testEnvironmentConfiguration);

app.Run();