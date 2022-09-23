using Serilog;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Kysect.Shreks.Tests.Tools;

public static class LogInitialization
{
    public static void Init()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .CreateLogger();
    }

    public static ILogger GetLogger()
    {
        return new SerilogLoggerFactory(Log.Logger).CreateLogger("TestLogger");
    }
}