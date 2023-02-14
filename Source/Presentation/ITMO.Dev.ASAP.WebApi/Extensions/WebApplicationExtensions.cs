using Serilog;
using static ITMO.Dev.ASAP.WebApi.Configuration.LoggerPathConfiguration;

namespace ITMO.Dev.ASAP.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseRequestLogging(this WebApplication webApplication)
    {
        return webApplication.UseSerilogRequestLogging(options =>
        {
            options.IncludeQueryInRequestPath = true;
            options.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(
                    Path.Join(BaseDirectoryName, "RequestLog_.log"),
                    outputTemplate: "{Timestamp:o} {Message}{NewLine}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30)
                .CreateLogger();
        });
    }
}