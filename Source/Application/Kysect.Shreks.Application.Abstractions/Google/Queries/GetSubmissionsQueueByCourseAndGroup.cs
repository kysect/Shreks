using Kysect.Shreks.Application.Dto.Tables;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public static class GetSubmissionsQueueByCourseAndGroup
{
    public record Query(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Response>;

    public record Response(SubmissionsQueueDto Queue);
}