using Kysect.Shreks.GithubIntegration.Extensions;
using Kysect.Shreks.GithubIntegration.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks.AspNetCore;

namespace Kysect.Shreks.GithubIntegration;

public class Startup
{
    public Startup()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        GithubConfiguration = Configuration.GetSection(nameof(GithubConfiguration)).Get<GithubConfiguration>();
    }

    public IConfiguration Configuration { get; init; }
    public GithubConfiguration GithubConfiguration { get; init; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddGithubServices(GithubConfiguration)
            .AddWebhookProcessors();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseGithubIntegration(GithubConfiguration);
    }
}