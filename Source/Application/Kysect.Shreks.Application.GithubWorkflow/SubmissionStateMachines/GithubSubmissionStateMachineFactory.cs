using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class GithubSubmissionStateMachineFactory
{
    private readonly IShreksDatabaseContext _context;
    private readonly SubmissionService _shreksCommandProcessor;

    public GithubSubmissionStateMachineFactory(IShreksDatabaseContext context, SubmissionService shreksCommandProcessor)
    {
        _context = context;
        _shreksCommandProcessor = shreksCommandProcessor;
    }

    public IGithubSubmissionStateMachine Create(ShreksCommandProcessor commandProcessor, ILogger logger, IPullRequestEventNotifier eventNotifier)
    {
        return new ReviewOnlyGithubSubmissionStateMachine(_context, _shreksCommandProcessor, commandProcessor, logger, eventNotifier);
    }
}