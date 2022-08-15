using Kysect.Shreks.Core.Users;
using MediatR;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class BaseContext
{
    public IMediator Mediator;
    public User Issuer;

    public BaseContext(IMediator mediator, User issuer)
    {
        Mediator = mediator;
        Issuer = issuer;
    }
}