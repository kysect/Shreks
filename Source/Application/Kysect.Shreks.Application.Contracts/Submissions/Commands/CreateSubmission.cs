using MediatR;

namespace Kysect.Shreks.Application.Contracts.Submissions.Commands;

internal static class CreateSubmission
{
    public record Command : IRequest<Response>;

    public record Response;
}