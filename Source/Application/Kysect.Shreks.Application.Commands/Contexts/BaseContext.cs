using Kysect.Shreks.Core.Users;
using MediatR;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class BaseContext
{
    public IMediator Mediator { get; }
    
    public CancellationToken CancellationToken { get; }
    public User Issuer { get; }

    public BaseContext(IMediator mediator, User issuer, CancellationToken cancellationToken)
    {
        Mediator = mediator;
        Issuer = issuer;
        CancellationToken = cancellationToken;
    }
}