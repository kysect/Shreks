using Kysect.Shreks.Application.Commands.Extensions;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.Google.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Playground.Github.TestEnv;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File("ShreksGithubPlayground.log")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var cacheConfiguration = builder.Configuration.GetSection(nameof(CacheConfiguration)).Get<CacheConfiguration>();
var githubIntegrationConfiguration = builder.Configuration.GetSection(nameof(GithubIntegrationConfiguration)).Get<GithubIntegrationConfiguration>();
var testEnvironmentConfiguration = builder.Configuration.GetSection(nameof(TestEnvironmentConfiguration)).Get<TestEnvironmentConfiguration>();

builder.Services
    .AddApplicationConfiguration()
    .AddMappingConfiguration()
    .AddHandlers()
    .AddApplicationCommands()
    .AddGithubServices(cacheConfiguration, githubIntegrationConfiguration)
    .AddGithubPlaygroundDatabase(testEnvironmentConfiguration)
    .AddDummyGoogleIntegration();

builder.Services
    .AddLogging(logBuilder => logBuilder.AddSerilog());

var app = builder.Build();

app.UseGithubIntegration(githubIntegrationConfiguration);
await app.Services.UseTestEnv(testEnvironmentConfiguration);

app.Run();