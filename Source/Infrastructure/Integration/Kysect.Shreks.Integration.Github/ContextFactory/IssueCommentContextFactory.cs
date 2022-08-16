using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Commands.Contexts;
using MediatR;
using Octokit.Webhooks.Events;

namespace Kysect.Shreks.Integration.Github.ContextFactory;

public class IssueCommentContextFactory : ICommandContextFactory
{
    private readonly IMediator _mediator;
    private readonly IssueCommentEvent _event;
    private readonly IShreksDatabaseContext _databaseContext;

    public IssueCommentContextFactory(IMediator mediator, IssueCommentEvent issueCommentEvent, IShreksDatabaseContext databaseContext)
    {
        _mediator = mediator;
        _event = issueCommentEvent;
        _databaseContext = databaseContext;
    }

    public async Task<BaseContext> CreateBaseContext()
    {
        var login = _event.Sender!.Login; //user is always present in this event
        var query = new GetUserByUsername.Query(login);
        var response = await _mediator.Send(query, cancellationToken);
        var issuer = await _databaseContext.Users.GetByIdAsync(response.UserId, cancellationToken);
        return new BaseContext(_mediator, issuer, cancellationToken);
    }

    public Task<SubmissionContext> CreateSubmissionContext()
    {
        //TODO: need pr -> submission query
        throw new NotImplementedException();
    }
}