using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Integration.Github.Client;
using MediatR;
using Octokit.Webhooks.Events;

namespace Kysect.Shreks.Integration.Github.ContextFactory;

public class IssueCommentContextFactory : ICommandContextFactory
{
    private readonly IMediator _mediator;
    private readonly IssueCommentEvent _event;
    private readonly IInstallationClientFactory _installationClientFactory;

    public IssueCommentContextFactory(IMediator mediator, IssueCommentEvent issueCommentEvent, IInstallationClientFactory installationClientFactory)
    {
        _mediator = mediator;
        _event = issueCommentEvent;
        _installationClientFactory = installationClientFactory;
    }

    public async Task<BaseContext> CreateBaseContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);
        
        return new BaseContext(_mediator, userId);
    }

    public async Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken)
    {
        var userId = await GetUserId(cancellationToken);

        ArgumentNullException.ThrowIfNull(_event.Repository);
        ArgumentNullException.ThrowIfNull(_event.Organization);
        var organizationName = _event.Organization.Login;
        var repositoryName = _event.Repository.Name;
        var issueNumber = _event.Issue.Number;

        var githubPullRequestDescriptor = new GithubPullRequestDescriptor(
            Payload: null,
            organizationName,
            repositoryName,
            BranchName: null,
            issueNumber);

        var submissionQuery = new GetCurrentUnratedSubmissionByPrNumber.Query(githubPullRequestDescriptor);
        var submissionResponse = await _mediator.Send(submissionQuery, cancellationToken);

        return new SubmissionContext(_mediator, userId, submissionResponse.SubmissionDto);
    }

    private async Task<Guid> GetUserId(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_event.Sender);
        var login = _event.Sender.Login; 
        var query = new GetUserByUsername.Query(login);
        var response = await _mediator.Send(query, cancellationToken);
        return response.UserId;
    }
}