using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;

namespace Kysect.Shreks.Application.GithubWorkflow;

public class PullRequestCommentContextFactory : ICommandContextFactory
{
    private readonly GithubPullRequestDescriptor _pullRequestDescriptor;
    private readonly GithubCommandSubmissionFactory _githubCommandSubmissionFactory;
    private readonly IShreksDatabaseContext _context;
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly SubmissionService _submissionService;

    public PullRequestCommentContextFactory(
        GithubPullRequestDescriptor pullRequestDescriptor,
        GithubSubmissionFactory githubSubmissionFactory,
        IShreksDatabaseContext context,
        SubmissionService submissionService)
    {
        _pullRequestDescriptor = pullRequestDescriptor;
        _context = context;
        _submissionService = submissionService;
        _githubCommandSubmissionFactory = new GithubCommandSubmissionFactory(githubSubmissionFactory);
        _githubSubmissionService = new GithubSubmissionService(_context);
    }

    public async Task<BaseContext> CreateBaseContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId();

        return new BaseContext(userId);
    }

    public async Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken)
    {
        Guid userId = await GetUserId();
        Submission submission = await _githubSubmissionService.GetCurrentUnratedSubmissionByPrNumber(_pullRequestDescriptor, cancellationToken);
        return new SubmissionContext(userId, submission.Id, _submissionService);
    }

    public async Task<UpdateContext> CreateUpdateContext(CancellationToken cancellationToken)
    {
        Guid userId = await GetUserId();
        SubjectCourse subjectCourse = await _context.SubjectCourseAssociations.GetSubjectCourseByOrganization(_pullRequestDescriptor.Organization, cancellationToken);
        Assignment assignment = await _githubSubmissionService.GetAssignmentByBranchAndSubjectCourse(subjectCourse.Id, _pullRequestDescriptor, cancellationToken);
        User student = await _context.UserAssociations.GetUserByGithubUsername(_pullRequestDescriptor.Repository);
        return new UpdateContext(userId, student, assignment, _submissionService);
    }

    public async Task<PullRequestContext> CreatePullRequestContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId();

        return new PullRequestContext(userId, _pullRequestDescriptor);
    }

    public async Task<PullRequestAndAssignmentContext> CreatePullRequestAndAssignmentContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId();

        SubjectCourse subjectCourse = await _context.SubjectCourseAssociations.GetSubjectCourseByOrganization(_pullRequestDescriptor.Organization, cancellationToken);
        Assignment assignment = await _githubSubmissionService.GetAssignmentByBranchAndSubjectCourse(subjectCourse.Id, _pullRequestDescriptor, cancellationToken);

        return new PullRequestAndAssignmentContext(_githubCommandSubmissionFactory, _pullRequestDescriptor, userId, assignment.Id);
    }

    private async Task<Guid> GetUserId()
    {
        User user = await _context.UserAssociations.GetUserByGithubUsername(_pullRequestDescriptor.Sender);
        return user.Id;
    }
}