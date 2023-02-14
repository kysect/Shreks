using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries;

internal static class GetSubjectCourseById
{
    public record Query(Guid Id) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}