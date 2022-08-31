using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Identity.Commands;

public static class Register
{
    public record struct Command(string Username, string Password) : IRequest;
}