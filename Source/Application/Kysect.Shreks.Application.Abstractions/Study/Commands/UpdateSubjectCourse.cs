using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Commands;

public static class UpdateSubjectCourse
{
    public record Command(Guid Id, string NewTitle) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}