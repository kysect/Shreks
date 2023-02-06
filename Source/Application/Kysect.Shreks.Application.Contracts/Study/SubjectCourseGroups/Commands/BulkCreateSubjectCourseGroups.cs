using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.SubjectCourseGroups.Commands;

public static class BulkCreateSubjectCourseGroups
{
    public record Command(Guid SubjectCourseId, IReadOnlyCollection<Guid> GroupIds) : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectCourseGroupDto> Groups);
}