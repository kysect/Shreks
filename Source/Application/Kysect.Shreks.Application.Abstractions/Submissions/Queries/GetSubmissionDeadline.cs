using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Queries;

public static class GetSubmissionDeadline
{
    public record Query(Guid SubmissionId) : IRequest<Response>;

    public record Response(DateOnly Deadline);
}