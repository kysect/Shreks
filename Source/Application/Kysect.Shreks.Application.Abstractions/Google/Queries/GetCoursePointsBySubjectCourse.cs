using Kysect.Shreks.Application.Abstractions.Google.Models;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public static class GetCoursePointsBySubjectCourse
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(CoursePoints Points);
}