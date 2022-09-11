using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Queries;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow;

public class PullRequestCommentContextFactory : ICommandContextFactory
{
    private readonly IMediator _mediator;
    private readonly ILogger _log;
    private readonly GithubPullRequestDescriptor _pullRequestDescriptor;
    private readonly GithubCommandSubmissionFactory _githubCommandSubmissionFactory;
    private readonly IShreksDatabaseContext _context;

    public PullRequestCommentContextFactory(
        IMediator mediator,
        GithubPullRequestDescriptor pullRequestDescriptor,
        ILogger log,
        GithubSubmissionFactory githubSubmissionFactory,
        IShreksDatabaseContext context)
    {
        _mediator = mediator;
        _pullRequestDescriptor = pullRequestDescriptor;
        _log = log;
        _context = context;
        _githubCommandSubmissionFactory = new GithubCommandSubmissionFactory(githubSubmissionFactory);
    }

    public async Task<BaseContext> CreateBaseContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        return new BaseContext(_mediator, _log, userId);
    }

    public async Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        var submissionQuery = new GetCurrentUnratedSubmissionByPrNumber.Query(_pullRequestDescriptor);
        var submissionResponse = await _mediator.Send(submissionQuery, cancellationToken);

        return new SubmissionContext(_mediator, _log, userId, submissionResponse.SubmissionDto);
    }

    public async Task<PullRequestContext> CreatePullRequestContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        return new PullRequestContext(_mediator, _log, userId, _pullRequestDescriptor);
    }

    public async Task<PullRequestAndAssignmentContext> CreatePullRequestAndAssignmentContext(CancellationToken cancellationToken)
    {
        var githubSubmissionService = new GithubSubmissionService(_context);
        var userId = await GetUserId(cancellationToken);

        SubjectCourse subjectCourse = await _context.SubjectCourseAssociations.GetSubjectCourseByOrganization(_pullRequestDescriptor.Organization, cancellationToken);
        Assignment assignment = await githubSubmissionService.GetAssignmentByBranchAndSubjectCourse(subjectCourse.Id, _pullRequestDescriptor, cancellationToken);

        return new PullRequestAndAssignmentContext(_mediator, userId, _pullRequestDescriptor, assignment.Id, _log, _githubCommandSubmissionFactory);
    }

    private async Task<Guid> GetUserId(CancellationToken cancellationToken)
    {
        User user = await _context.UserAssociations.FindUserByGithubUsername(_pullRequestDescriptor.Sender)
                                        ?? throw new EntityNotFoundException($"Entity of type User with login {_pullRequestDescriptor.Sender}");
        return user.Id;
    }
}