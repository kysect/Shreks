using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Queries;

internal static class GetSubjectCourseId
{
    public record Query(string OrganizationName) : IRequest<Response>;

    public record Response(Guid Id);
}