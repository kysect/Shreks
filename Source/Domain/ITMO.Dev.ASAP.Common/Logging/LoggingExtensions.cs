using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Common.Logging;

public static class LoggingExtensions
{
    public static ILogger WithPrefix(this ILogger logger, string prefix)
    {
        return new PrefixLoggerProxy(logger, prefix);
    }
}