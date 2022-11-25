using MediatR;

namespace Kysect.Shreks.Application.Contracts.Google.Commands;

public static class UpdateSubjectGroupQueueSheet
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Unit>;
}