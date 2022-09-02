using Kysect.Shreks.Integration.Github.Helpers;
using Microsoft.AspNetCore.Builder;
using Octokit.Webhooks.AspNetCore;

namespace Kysect.Shreks.Integration.Github.Extensions;

public static class AppBuilderExtensions
{
    public static IApplicationBuilder UseGithubIntegration(this IApplicationBuilder app, GithubIntegrationConfiguration githubIntegrationConfiguration)
    {
        ArgumentNullException.ThrowIfNull(githubIntegrationConfiguration.GithubAuthConfiguration.GithubAppSecret);

        string appSecret = githubIntegrationConfiguration.GithubAuthConfiguration.GithubAppSecret;

        app.UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapGitHubWebhooks(secret: appSecret);
            });

        return app;
    }
}