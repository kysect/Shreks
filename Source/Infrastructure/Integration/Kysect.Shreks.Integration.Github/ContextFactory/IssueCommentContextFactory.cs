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

    public Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken)
    {
        //TODO: need pr -> submission query
        throw new NotImplementedException();
    }
}