using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Students;

public static class GetGithubUsernamesForSubjectCourse
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<string> StudentGithubUsernames);
}