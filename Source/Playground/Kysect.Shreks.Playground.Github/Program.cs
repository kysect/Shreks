using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;

var builder = WebApplication.CreateBuilder(args);

var shreksConfiguration = builder.Configuration.GetSection(nameof(ShreksConfiguration)).Get<ShreksConfiguration>();
shreksConfiguration.Verify();

builder.Services
    .AddGithubServices(shreksConfiguration)
    .AddWebhookProcessors();

var app = builder.Build();

app.UseGithubIntegration(shreksConfiguration);

app.Run();