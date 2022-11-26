using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Commands;

internal class DeleteSubjectCourseGroup
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Unit>;
}