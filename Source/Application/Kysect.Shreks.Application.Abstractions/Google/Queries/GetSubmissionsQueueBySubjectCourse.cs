using Kysect.Shreks.Application.Dto.Tables;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public class GetSubmissionsQueueBySubjectCourse
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(SubmissionsQueueDto Queue);
}