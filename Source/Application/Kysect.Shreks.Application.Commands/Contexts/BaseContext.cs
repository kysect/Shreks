using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class BaseContext
{
    public ILogger Log { get; }
    public Guid IssuerId { get; }

    public BaseContext(ILogger log, Guid issuerId)
    {
        Log = log;
        IssuerId = issuerId;
    }
}