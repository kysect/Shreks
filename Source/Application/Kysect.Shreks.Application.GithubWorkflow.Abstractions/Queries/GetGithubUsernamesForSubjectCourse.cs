using MediatR;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Queries;

public static class GetGithubUsernamesForSubjectCourse
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<string> StudentGithubUsernames);
}