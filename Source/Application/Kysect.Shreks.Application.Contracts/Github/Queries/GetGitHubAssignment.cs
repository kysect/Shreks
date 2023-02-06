using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Queries;

internal static class GetGitHubAssignment
{
    public record Query(string OrganizationName, string BranchName) : IRequest<Response>;

    public record Response(AssignmentDto Assignment);
}