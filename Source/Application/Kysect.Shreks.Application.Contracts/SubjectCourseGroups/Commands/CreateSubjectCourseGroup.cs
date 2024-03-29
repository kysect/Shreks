using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.SubjectCourseGroups.Commands;

internal static class CreateSubjectCourseGroup
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Response>;

    public record Response(SubjectCourseGroupDto SubjectCourseGroup);
}