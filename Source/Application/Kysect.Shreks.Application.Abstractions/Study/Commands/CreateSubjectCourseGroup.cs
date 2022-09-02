using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Commands;

public static class CreateSubjectCourseGroup
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Response>;

    public record Response(SubjectCourseGroupDto SubjectCourseGroup);
}