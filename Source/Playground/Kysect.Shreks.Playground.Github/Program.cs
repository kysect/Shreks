using System.Reflection;
using Kysect.Shreks.Application.Commands.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using MediatR;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File("ShreksGithubPlayground.log")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var shreksConfiguration = configuration.GetSection(nameof(ShreksConfiguration)).Get<ShreksConfiguration>();
//shreksConfiguration.AppendSecret(builder.Configuration["GithubAppSecret"]).Verify();
TestEnvConfiguration testEnvConfiguration = configuration.GetSection(nameof(TestEnvConfiguration)).Get<TestEnvConfiguration>();

builder.Services
    .AddMappingConfiguration()
    .AddDatabaseContext(optionsBuilder => optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("postgres"))
            .UseLazyLoadingProxies())
    .AddHandlers()
    .AddApplicationCommands()
    .AddGithubServices(shreksConfiguration)
    .SetupTestEnv(testEnvConfiguration);

builder.Services
    .AddLogging(logBuilder => logBuilder.AddSerilog());

var app = builder.Build();

app.UseGithubIntegration(shreksConfiguration);
await app.Services.UseTestEnv(testEnvConfiguration);

app.Run();