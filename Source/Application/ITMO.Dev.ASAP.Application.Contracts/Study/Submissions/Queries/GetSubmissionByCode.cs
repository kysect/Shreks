using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Queries;

internal static class GetSubmissionByCode
{
    public record Query(Guid StudentId, Guid AssignmentId, int Code) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}