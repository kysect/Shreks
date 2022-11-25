using MediatR;

namespace Kysect.Shreks.Application.Contracts.Identity.Commands;

public static class Register
{
    public record struct Command(string Username, string Password) : IRequest;
}