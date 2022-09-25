using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Kysect.Shreks.Tests.Tools;

public static class LogInitialization
{
    public static ILogger InitTestLogger(ITestOutputHelper output)
    {
        Logger logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.TestOutput(output)
            .CreateLogger();

        Log.Logger = logger;
        return new SerilogLoggerFactory(logger).CreateLogger("TestLogger");
    }
}