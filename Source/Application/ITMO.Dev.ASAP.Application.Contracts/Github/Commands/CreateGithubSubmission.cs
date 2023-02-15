using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal static class CreateGithubSubmission
{
    public record Command(
        Guid IssuerId,
        Guid AssignmentId,
        string OrganizationName,
        string RepositoryName,
        long PullRequestNumber,
        string Payload) : IRequest<Response>;

    public record Response(SubmissionRateDto Submission);
}