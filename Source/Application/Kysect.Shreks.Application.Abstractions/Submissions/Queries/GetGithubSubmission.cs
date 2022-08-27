using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Queries;

public static class GetGithubSubmission
{
    public record Query(string Organization, string Repository, long PrNumber) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}