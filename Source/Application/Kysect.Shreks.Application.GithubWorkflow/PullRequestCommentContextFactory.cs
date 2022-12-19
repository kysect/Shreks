using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
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
    private readonly ISubmissionService _submissionService;

    public PullRequestCommentContextFactory(
        GithubPullRequestDescriptor pullRequestDescriptor,
        GithubSubmissionFactory githubSubmissionFactory,
        IShreksDatabaseContext context,
        ISubmissionService submissionService)
    {
        _pullRequestDescriptor = pullRequestDescriptor;
        _context = context;
        _submissionService = submissionService;

        _githubCommandSubmissionFactory = new GithubCommandSubmissionFactory(
            githubSubmissionFactory, pullRequestDescriptor);

        _githubSubmissionService = new GithubSubmissionService(_context);
    }

    public async Task<BaseContext> CreateBaseContext(CancellationToken cancellationToken)
    {
        Guid userId = await GetUserId();

        return new BaseContext(userId);
    }

    public async Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken)
    {
        Guid userId = await GetUserId();

        Submission submission = await _githubSubmissionService.GetCurrentUnratedSubmissionByPrNumber(
            _pullRequestDescriptor, cancellationToken);

        return new SubmissionContext(userId, submission.Id, _submissionService);
    }

    public async Task<UpdateContext> CreateUpdateContextAsync(CancellationToken cancellationToken)
    {
        Guid userId = await GetUserId();

        SubjectCourse subjectCourse = await _context.SubjectCourseAssociations.GetSubjectCourseByOrganization(
            _pullRequestDescriptor.Organization, cancellationToken);

        Assignment assignment = await _githubSubmissionService.GetAssignmentByBranchAndSubjectCourse(
            subjectCourse.Id, _pullRequestDescriptor, cancellationToken);

        User student = await _context.UserAssociations.GetUserByGithubUsername(_pullRequestDescriptor.Repository);
        return new UpdateContext(userId, student, assignment, _submissionService, GetDefaultSubmissionFactory());
    }

    public async Task<PayloadAndAssignmentContext> CreatePayloadAndAssignmentContext(CancellationToken cancellationToken)
    {
        Guid userId = await GetUserId();

        SubjectCourse subjectCourse = await _context.SubjectCourseAssociations.GetSubjectCourseByOrganization(
            _pullRequestDescriptor.Organization, cancellationToken);

        Assignment assignment = await _githubSubmissionService.GetAssignmentByBranchAndSubjectCourse(
            subjectCourse.Id, _pullRequestDescriptor, cancellationToken);

        return new PayloadAndAssignmentContext
        (
            userId,
            _githubCommandSubmissionFactory,
            assignment.Id,
            _pullRequestDescriptor.Payload
        );
    }

    private async Task<Guid> GetUserId()
    {
        User user = await _context.UserAssociations.GetUserByGithubUsername(_pullRequestDescriptor.Sender);
        return user.Id;
    }

    private Func<Task<Submission>> GetDefaultSubmissionFactory()
    {
        return async () => await _githubSubmissionService.GetLastSubmissionByPr(
            _pullRequestDescriptor);
    }
}