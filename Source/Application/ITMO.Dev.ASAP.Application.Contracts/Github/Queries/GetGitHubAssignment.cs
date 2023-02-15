using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Queries;

internal static class GetGitHubAssignment
{
    public record Query(string OrganizationName, string BranchName) : IRequest<Response>;

    public record Response(AssignmentDto Assignment);
}