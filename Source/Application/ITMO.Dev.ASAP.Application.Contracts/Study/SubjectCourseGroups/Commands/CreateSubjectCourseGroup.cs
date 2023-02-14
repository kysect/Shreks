using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands;

internal static class CreateSubjectCourseGroup
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Response>;

    public record Response(SubjectCourseGroupDto SubjectCourseGroup);
}