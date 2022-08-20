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
        var login = _event.Sender!.Login; //user is always present in this event
        var query = new GetUserByUsername.Query(login);
        var response = await _mediator.Send(query, cancellationToken);

        return new BaseContext(_mediator, response.UserId);
    }

    public async Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken)
    {
        var login = _event.Sender!.Login; //user is always present in this event
        var userQuery = new GetUserByUsername.Query(login);
        var userResponse = await _mediator.Send(userQuery, cancellationToken);

        var submissionQuery = new GetCurrentUnratedSubmissionByPrNumber.Query(_event.Organization!.Login, 
            _event.Repository!.Name, _event.Issue.Number);
        var submissionResponse =  await _mediator.Send(submissionQuery, cancellationToken);

        return new SubmissionContext(_mediator, userResponse.UserId, submissionResponse.SubmissionDto);
    }
}