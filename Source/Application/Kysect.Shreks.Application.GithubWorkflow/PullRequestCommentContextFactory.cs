using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow;

public class PullRequestCommentContextFactory : ICommandContextFactory
{
    private readonly ILogger _log;
    private readonly GithubPullRequestDescriptor _pullRequestDescriptor;
    private readonly GithubCommandSubmissionFactory _githubCommandSubmissionFactory;
    private readonly IShreksDatabaseContext _context;
    private readonly GithubSubmissionService _githubSubmissionService;

    public PullRequestCommentContextFactory(GithubPullRequestDescriptor pullRequestDescriptor,
        ILogger log,
        GithubSubmissionFactory githubSubmissionFactory,
        IShreksDatabaseContext context)
    {
        _pullRequestDescriptor = pullRequestDescriptor;
        _log = log;
        _context = context;
        _githubCommandSubmissionFactory = new GithubCommandSubmissionFactory(githubSubmissionFactory);
        _githubSubmissionService = new GithubSubmissionService(_context);
    }

    public async Task<BaseContext> CreateBaseContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        return new BaseContext(_log, userId);
    }

    public async Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken)
    {
        Guid userId = await GetUserId(cancellationToken);
        Submission submission = await _githubSubmissionService.GetCurrentUnratedSubmissionByPrNumber(_pullRequestDescriptor, cancellationToken);
        return new SubmissionContext(_log, userId, submission);
    }

    public async Task<PullRequestContext> CreatePullRequestContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        return new PullRequestContext(_log, userId, _pullRequestDescriptor);
    }

    public async Task<PullRequestAndAssignmentContext> CreatePullRequestAndAssignmentContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        SubjectCourse subjectCourse = await _context.SubjectCourseAssociations.GetSubjectCourseByOrganization(_pullRequestDescriptor.Organization, cancellationToken);
        Assignment assignment = await _githubSubmissionService.GetAssignmentByBranchAndSubjectCourse(subjectCourse.Id, _pullRequestDescriptor, cancellationToken);

        return new PullRequestAndAssignmentContext(_githubCommandSubmissionFactory, _pullRequestDescriptor, userId, assignment.Id, _log);
    }

    private async Task<Guid> GetUserId(CancellationToken cancellationToken)
    {
        User user = await _context.UserAssociations.FindUserByGithubUsername(_pullRequestDescriptor.Sender)
                                        ?? throw new EntityNotFoundException($"Entity of type User with login {_pullRequestDescriptor.Sender}");
        return user.Id;
    }
}