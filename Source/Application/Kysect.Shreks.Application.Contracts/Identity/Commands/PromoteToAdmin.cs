using MediatR;

namespace Kysect.Shreks.Application.Contracts.Identity.Commands;

public static class PromoteToAdmin
{
    public record struct Command(string Username) : IRequest;
}