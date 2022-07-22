using Kysect.Shreks.GithubIntegration.Helpers;
using Microsoft.AspNetCore.Builder;
using Octokit.Webhooks.AspNetCore;

namespace Kysect.Shreks.GithubIntegration.Extensions;

public static class AppBuilderExtensions
{
    public static IApplicationBuilder UseGithubIntegration(this IApplicationBuilder app, ShreksConfiguration shreksConfiguration)
    {
        var gitHubConf = shreksConfiguration.GithubConfiguration;

        app.UseRouting()
            .UseEndpoints(endpoints => endpoints.MapGitHubWebhooks(secret: gitHubConf.Secret));

        return app;
    }
}