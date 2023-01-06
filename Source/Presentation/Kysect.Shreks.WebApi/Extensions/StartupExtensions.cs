using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;

namespace Kysect.Shreks.WebApi.Extensions;

internal static class StartupExtensions
{
    internal static WebApplication Configure(
        this WebApplication app,
        GithubIntegrationConfiguration githubIntegrationConfiguration)
    {
        app.UseRequestLogging();

        if (app.Environment.IsDevelopment())
            app.UseWebAssemblyDebugging();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

        app
            .UseBlazorFrameworkFiles()
            .UseStaticFiles()
            .UseRouting();

        app.MapRazorPages();

        app
            .UseAuthentication()
            .UseAuthorization();

        app.MapFallbackToFile("index.html");

        app.MapControllers();
        app.UseGithubIntegration(githubIntegrationConfiguration);

        return app;
    }
}