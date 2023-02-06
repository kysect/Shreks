using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Submissions.Commands;

internal static class CreateSubmission
{
    public record Command : IRequest<Response>;

    public record Response;
}