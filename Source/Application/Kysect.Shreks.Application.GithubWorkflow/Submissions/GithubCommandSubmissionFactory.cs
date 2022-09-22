using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;

namespace Kysect.Shreks.Application.GithubWorkflow.Submissions;

public class GithubCommandSubmissionFactory : ICommandSubmissionFactory
{
    private readonly IShreksDatabaseContext _context;
    private readonly GithubSubmissionFactory _githubSubmissionFactory;
    private readonly GithubPullRequestDescriptor _pullRequestDescriptor;

    public GithubCommandSubmissionFactory(GithubSubmissionFactory githubSubmissionFactory, IShreksDatabaseContext context, GithubPullRequestDescriptor pullRequestDescriptor)
    {
        _githubSubmissionFactory = githubSubmissionFactory;
        _context = context;
        _pullRequestDescriptor = pullRequestDescriptor;
    }

    public async Task<SubmissionRateDto> CreateSubmission(Guid userId, Guid assignmentId)
    {
        GithubSubmission submission = await _githubSubmissionFactory.CreateGithubSubmissionAsync(userId, assignmentId, _pullRequestDescriptor, CancellationToken.None);
        return SubmissionRateDtoFactory.CreateFromSubmission(submission);
    }
}