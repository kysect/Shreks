using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Dto.Github;
using MediatR;

namespace Kysect.Shreks.Integration.Github.ContextFactory;

public class PullRequestCommentContextFactory : ICommandContextFactory
{
    private readonly IMediator _mediator;
    private readonly GithubPullRequestDescriptor _pullRequestDescriptor;

    public PullRequestCommentContextFactory(
        IMediator mediator,
        GithubPullRequestDescriptor pullRequestDescriptor)
    {
        _mediator = mediator;
        _pullRequestDescriptor = pullRequestDescriptor;
    }

    public async Task<BaseContext> CreateBaseContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        return new BaseContext(_mediator, userId);
    }

    public async Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        var submissionQuery = new GetCurrentUnratedSubmissionByPrNumber.Query(_pullRequestDescriptor);
        var submissionResponse = await _mediator.Send(submissionQuery, cancellationToken);

        return new SubmissionContext(_mediator, userId, submissionResponse.SubmissionDto);
    }

    public async Task<PullRequestContext> CreatePullRequestContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        return new PullRequestContext(_mediator, userId, _pullRequestDescriptor);
    }

    public async Task<PullRequestAndAssignmentContext> CreatePullRequestAndAssignmentContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        var subjectCourseId = await GetSubjectCourseByOrganization(_pullRequestDescriptor.Organization, cancellationToken);
        var assignmentId = await GetAssignemntByBranchAndSubjectCourse(_pullRequestDescriptor.BranchName, subjectCourseId, cancellationToken);
        return new PullRequestAndAssignmentContext(_mediator, userId, _pullRequestDescriptor, assignmentId);
    }

    private async Task<Guid> GetSubjectCourseByOrganization(string organization, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSubjectCourseByOrganization.Query(organization));
        return response.SubjectCourseId;
    }

    private async Task<Guid> GetAssignemntByBranchAndSubjectCourse(string branch, Guid subjectCourseId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAssignmentByBranchAndSubjectCourse.Query(branch,
                subjectCourseId));
        return response.AssignmentId;
    }

    private async Task<Guid> GetUserId(CancellationToken cancellationToken)
    {
        var query = new GetUserByGithubUsername.Query(_pullRequestDescriptor.Sender);
        var response = await _mediator.Send(query, cancellationToken);
        return response.UserId;
    }
}