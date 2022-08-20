using MediatR;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class BaseContext
{
    public IMediator Mediator { get; }
    public Guid IssuerId { get; }

    public BaseContext(IMediator mediator, Guid issuerId)
    {
        Mediator = mediator;
        IssuerId = issuerId;
    }
}