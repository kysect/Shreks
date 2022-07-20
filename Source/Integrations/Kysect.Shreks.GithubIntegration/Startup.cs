using Kysect.Shreks.GithubIntegration.Extensions;
using Kysect.Shreks.GithubIntegration.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks.Models;

namespace Kysect.Shreks.GithubIntegration;

public class Startup
{
    public Startup()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        ShreksConfiguration = Configuration.GetSection(nameof(ShreksConfiguration)).Get<ShreksConfiguration>();
    }

    public IConfiguration Configuration { get; init; }
    public ShreksConfiguration ShreksConfiguration { get; init; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddGithubServices(ShreksConfiguration)
            .AddWebhookProcessors();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseGithubIntegration(ShreksConfiguration);
    }
}