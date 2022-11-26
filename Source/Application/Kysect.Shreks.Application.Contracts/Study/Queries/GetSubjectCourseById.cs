using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

internal static class GetSubjectCourseById
{
    public record Query(Guid Id) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}