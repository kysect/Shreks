using Kysect.Shreks.Application.Commands.Extensions;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Playground.Github.TestEnv;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File("ShreksGithubPlayground.log")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
ShreksConfiguration shreksConfiguration = builder.Configuration.GetShreksConfiguration();
TestEnvConfiguration testEnvConfiguration = configuration.GetSection(nameof(TestEnvConfiguration)).Get<TestEnvConfiguration>();

builder.Services
    .AddApplicationConfiguration()
    .AddMappingConfiguration()
    .AddHandlers()
    .AddApplicationCommands()
    .AddGithubServices(shreksConfiguration)
    .AddGithubPlaygroundDatabase(testEnvConfiguration)
    .AddDummyGoogleIntegration();

builder.Services
    .AddLogging(logBuilder => logBuilder.AddSerilog());

var app = builder.Build();

app.UseGithubIntegration(shreksConfiguration);
await app.Services.UseTestEnv(testEnvConfiguration);

app.Run();