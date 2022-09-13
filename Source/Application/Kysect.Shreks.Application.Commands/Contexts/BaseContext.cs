namespace Kysect.Shreks.Application.Commands.Contexts;

public class BaseContext
{
    public Guid IssuerId { get; }

    public BaseContext(Guid issuerId)
    {
        IssuerId = issuerId;
    }
}