using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Commands;

public class DeleteSubjectCourseGroup
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Unit>;
}