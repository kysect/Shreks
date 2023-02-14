using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;

internal static class Register
{
    public record struct Command(string Username, string Password) : IRequest;
}