using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Commands.Contexts;
using MediatR;
using Octokit.Webhooks.Events;

namespace Kysect.Shreks.Integration.Github.ContextFactory;

public class IssueCommentContextFactory : ICommandContextFactory
{
    private readonly IMediator _mediator;
    private readonly IssueCommentEvent _event;

    public IssueCommentContextFactory(IMediator mediator, IssueCommentEvent issueCommentEvent)
    {
        _mediator = mediator;
        _event = issueCommentEvent;
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
        var submissionQuery = new GetCurrentUnratedSubmissionByPrNumber.Query(
            organizationName, repositoryName, issueNumber);
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