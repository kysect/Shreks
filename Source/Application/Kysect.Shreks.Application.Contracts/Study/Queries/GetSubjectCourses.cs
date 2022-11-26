using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

public static class GetSubjectCourses
{
    public record Query : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectCourseDto> Subjects);
}