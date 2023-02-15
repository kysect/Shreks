using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands;

internal class DeleteSubjectCourseGroup
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Unit>;
}