using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class BaseContext
{
    public IMediator Mediator { get; }
    public ILogger Log { get; }
    public Guid IssuerId { get; }

    public BaseContext(IMediator mediator, ILogger log, Guid issuerId)
    {
        Mediator = mediator;
        Log = log;
        IssuerId = issuerId;
    }
}