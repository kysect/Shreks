using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Submissions.Queries;

internal static class GetLastSubmission
{
    public record Query(Guid StudentId, Guid AssignmentId) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}