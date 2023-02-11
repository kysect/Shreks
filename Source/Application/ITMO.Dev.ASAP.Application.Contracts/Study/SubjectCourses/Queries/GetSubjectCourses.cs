using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries;

internal static class GetSubjectCourses
{
    public record Query : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectCourseDto> Subjects);
}