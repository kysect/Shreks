using Serilog;
using Serilog.Events;
using static Kysect.Shreks.WebApi.Configuration.LoggerPathConfiguration;

namespace Kysect.Shreks.WebApi.Extensions;

public static class ConfigureHostBuilderExtensions {
    public static IHostBuilder UseSerilogForAppLogs(this ConfigureHostBuilder hostBuilder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .WriteTo
                .Console(outputTemplate: "[{Timestamp:T} {Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}")
            .WriteTo
                .File(Path.Join(BaseDirectoryName, "AppLogs_.log"),
                    outputTemplate: "{Timestamp:o} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30
                )
            .CreateLogger();

        return hostBuilder.UseSerilog();
    }
}