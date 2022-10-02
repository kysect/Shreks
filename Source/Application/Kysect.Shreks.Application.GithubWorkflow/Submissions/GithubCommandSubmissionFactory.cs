using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Models;

namespace Kysect.Shreks.Application.GithubWorkflow.Submissions;

public class GithubCommandSubmissionFactory : ICommandSubmissionFactory
{
    private readonly GithubSubmissionFactory _githubSubmissionFactory;
    private readonly GithubPullRequestDescriptor _pullRequestDescriptor;

    public GithubCommandSubmissionFactory(
        GithubSubmissionFactory githubSubmissionFactory,
        GithubPullRequestDescriptor pullRequestDescriptor)
    {
        _githubSubmissionFactory = githubSubmissionFactory;
        _pullRequestDescriptor = pullRequestDescriptor;
    }

    public async Task<SubmissionRateDto> CreateSubmission(Guid userId, Guid assignmentId)
    {
        GithubSubmissionCreationResult result = await _githubSubmissionFactory.CreateOrUpdateGithubSubmission(
            _pullRequestDescriptor, CancellationToken.None);

        return SubmissionRateDtoFactory.CreateFromSubmission(result.Submission);
    }
}