using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands;

internal static class UpdateSubjectCourse
{
    public record Command(Guid Id, string NewTitle) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}