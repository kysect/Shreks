using Kysect.Shreks.GithubIntegration.Extensions;
using Kysect.Shreks.GithubIntegration.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        CacheConfiguration = Configuration.GetSection(nameof(CacheConfiguration)).Get<CacheConfiguration>();
    }

    public IConfiguration Configuration { get; init; }
    public GithubConfiguration GithubConfiguration { get; init; }
    public CacheConfiguration CacheConfiguration { get; init; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddGithubServices(GithubConfiguration, CacheConfiguration)
            .AddWebhookProcessors();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseGithubIntegration(GithubConfiguration);
    }
}