using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Queries;

internal static class GetGitHubSubjectCourseId
{
    public record Query(string OrganizationName) : IRequest<Response>;

    public record Response(Guid Id);
}