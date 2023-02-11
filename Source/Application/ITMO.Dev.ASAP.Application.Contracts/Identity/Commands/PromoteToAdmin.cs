using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;

internal static class PromoteToAdmin
{
    public record struct Command(string Username) : IRequest;
}