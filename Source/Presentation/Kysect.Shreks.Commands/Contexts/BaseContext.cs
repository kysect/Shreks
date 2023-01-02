using MediatR;

namespace Kysect.Shreks.Commands.Contexts;

public class BaseContext
{
    public BaseContext(Guid issuerId, IMediator mediator)
    {
        IssuerId = issuerId;
        Mediator = mediator;
    }

    public Guid IssuerId { get; }
    public IMediator Mediator { get; }
}