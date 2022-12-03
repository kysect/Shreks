using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

internal class GetSubjectCourseGroupsBySubjectCourseId
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectCourseGroupDto> Groups);
}