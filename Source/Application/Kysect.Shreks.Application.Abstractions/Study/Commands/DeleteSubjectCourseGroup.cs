using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Commands;

public class DeleteSubjectCourseGroup
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Unit>;
}