using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Identity.Commands;

public static class PromoteToAdmin
{
    public record struct Command(string Username) : IRequest;
}