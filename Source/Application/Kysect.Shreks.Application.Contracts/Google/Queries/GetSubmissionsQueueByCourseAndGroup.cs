using Kysect.Shreks.Application.Dto.Tables;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Google.Queries;

public static class GetSubjectCourseGroupSubmissionQueue
{
    public record Query(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Response>;

    public record Response(SubmissionsQueueDto Queue);
}