using MediatR;

namespace Kysect.Shreks.Application.Contracts.Google.Commands;

public static class UpdateSubjectCourseTable
{
    public record Command(Guid SubjectCourseId) : IRequest<Unit>;
}