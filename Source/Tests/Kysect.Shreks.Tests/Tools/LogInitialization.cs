using Serilog;

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
}