using MediatR;

namespace Kysect.Shreks.Application.Contracts.Identity.Commands;

internal static class PromoteToAdmin
{
    public record struct Command(string Username) : IRequest;
}