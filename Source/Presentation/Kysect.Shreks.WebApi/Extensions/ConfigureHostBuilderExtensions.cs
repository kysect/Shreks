using Serilog;

namespace Kysect.Shreks.WebApi.Extensions;

public static class ConfigureHostBuilderExtensions
{
    public static IHostBuilder UseSerilogForAppLogs(this ConfigureHostBuilder hostBuilder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        return hostBuilder.UseSerilog();
    }
}