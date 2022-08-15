using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Commands.Contexts;
using MediatR;
using Octokit.Webhooks.Events;

namespace Kysect.Shreks.Integration.Github.ContextCreators;

public class IssueCommentContextCreator : ICommandContextCreator
{
    private IMediator _mediator;
    private IssueCommentEvent _event;
    private IShreksDatabaseContext _databaseContext;

    public IssueCommentContextCreator(IMediator mediator, IssueCommentEvent issueCommentEvent, IShreksDatabaseContext databaseContext)
    {
        _mediator = mediator;
        _event = issueCommentEvent;
        _databaseContext = databaseContext;
    }

    public async Task<BaseContext> CreateBaseContext()
    {
        var respone = await _mediator.Send(new GetUserByUsername.Query(_event.Sender?.Login ?? throw new Exception()));
        return new BaseContext(_mediator, await _databaseContext.Users.GetByIdAsync(respone.UserId));
    }

    public Task<SubmissionContext> CreateSubmissionContext()
    {
        throw new NotImplementedException();
    }
}