using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Commands;

internal static class UpdateSubjectCourse
{
    public record Command(Guid Id, string NewTitle) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}