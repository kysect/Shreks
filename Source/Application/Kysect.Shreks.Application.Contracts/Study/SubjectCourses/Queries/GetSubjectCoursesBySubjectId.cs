using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.SubjectCourses.Queries;

internal static class GetSubjectCoursesBySubjectId
{
    public record Query(Guid SubjectId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectCourseDto> Courses);
}