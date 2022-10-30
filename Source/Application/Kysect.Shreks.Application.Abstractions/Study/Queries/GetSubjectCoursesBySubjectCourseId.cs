using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Queries;

public static class GetSubjectCoursesBySubjectCourseId
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectCourseDto> Courses);
}