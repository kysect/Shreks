using Kysect.Shreks.Common;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Common.Logging;

public static class LoggingExtensions
{
    public static ILogger WithPrefix(this ILogger logger, string prefix) => new PrefixLoggerProxy(logger, prefix);
}