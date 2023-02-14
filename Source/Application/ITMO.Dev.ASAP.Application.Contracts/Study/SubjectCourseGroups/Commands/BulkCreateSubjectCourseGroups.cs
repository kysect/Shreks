using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands;

internal static class BulkCreateSubjectCourseGroups
{
    public record Command(Guid SubjectCourseId, IReadOnlyCollection<Guid> GroupIds) : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectCourseGroupDto> Groups);
}