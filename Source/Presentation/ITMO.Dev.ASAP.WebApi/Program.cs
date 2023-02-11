using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.WebApi.Configuration;
using ITMO.Dev.ASAP.WebApi.Extensions;
using ITMO.Dev.ASAP.WebApi.Helpers;

namespace ITMO.Dev.ASAP.WebApi;

internal class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilogForAppLogs(builder.Configuration);

        var webApiConfiguration = new WebApiConfiguration(builder.Configuration);
        IConfigurationSection identityConfigurationSection =
            builder.Configuration.GetSection("Identity").GetSection("IdentityConfiguration");

        builder.Services.ConfigureServiceCollection(
            webApiConfiguration,
            identityConfigurationSection,
            builder.Environment.IsDevelopment());

        WebApplication app = builder.Build().Configure(webApiConfiguration.GithubIntegrationConfiguration);

        using (IServiceScope scope = app.Services.CreateScope())
        {
            await SeedingHelper.SeedAdmins(scope.ServiceProvider, app.Configuration);
            await scope.ServiceProvider.UseDatabaseContext();
        }

        await app.RunAsync();
    }
}