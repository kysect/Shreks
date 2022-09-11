using Kysect.Shreks.Application.Commands.Submissions;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.GithubWorkflow;

public class GithubCommandSubmissionFactory : ICommandSubmissionFactory
{
    private readonly GithubSubmissionFactory _githubSubmissionFactory;

    public GithubCommandSubmissionFactory(GithubSubmissionFactory githubSubmissionFactory)
    {
        _githubSubmissionFactory = githubSubmissionFactory;
    }

    public async Task<SubmissionRateDto> CreateSubmission(Guid userId, Guid assignmentId, GithubPullRequestDescriptor pullRequestDescriptor)
    {
        GithubSubmission submission = await _githubSubmissionFactory.CreateGithubSubmissionAsync(userId, assignmentId, pullRequestDescriptor, CancellationToken.None);
        return SubmissionRateDtoFactory.CreateFromSubmission(submission);
    }
}