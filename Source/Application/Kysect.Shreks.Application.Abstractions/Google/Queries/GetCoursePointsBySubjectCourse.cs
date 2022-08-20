using Kysect.Shreks.Application.Dto.Tables;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public static class GetCoursePointsBySubjectCourse
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(CoursePointsDto Points);
}