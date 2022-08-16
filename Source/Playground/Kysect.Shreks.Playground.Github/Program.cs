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

var shreksConfiguration = builder.Configuration.GetSection(nameof(ShreksConfiguration)).Get<ShreksConfiguration>();
shreksConfiguration.AppendSecret(builder.Configuration["GithubAppSecret"]).Verify();

builder.Services
    .AddMediatR(c => c.AsSingleton(), Assembly.GetExecutingAssembly())
    .AddGithubServices(shreksConfiguration)
    .AddApplicationCommands()
    .AddWebhookProcessors();

builder.Services
    .AddLogging(logBuilder => logBuilder.AddSerilog());

var app = builder.Build();

app.UseGithubIntegration(shreksConfiguration);

app.Run();