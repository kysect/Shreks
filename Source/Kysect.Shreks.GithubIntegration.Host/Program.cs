using Kysect.Shreks.GithubIntegration.Extensions;
using Kysect.Shreks.GithubIntegration.Helpers;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json")
    .AddEnvironmentVariables()
    .Build();

var shreksConfiguration = configuration.GetSection(nameof(ShreksConfiguration)).Get<ShreksConfiguration>();
shreksConfiguration.Verify();

builder.Services
    .AddGithubServices(shreksConfiguration)
    .AddWebhookProcessors();

var app = builder.Build();

app.UseGithubIntegration(shreksConfiguration);

app.Run();