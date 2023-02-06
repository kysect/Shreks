using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.SubjectCourseGroups.Commands;

internal class DeleteSubjectCourseGroup
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Unit>;
}