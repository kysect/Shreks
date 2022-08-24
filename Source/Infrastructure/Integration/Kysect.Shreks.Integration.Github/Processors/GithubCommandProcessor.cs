using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.Integration.Github.Processors;

//TODO: catch different exceptions and write better messages
public class GithubCommandProcessor : IShreksCommandVisitor<BaseShreksCommandResult>
{
    private readonly ICommandContextFactory _contextFactory;
    private readonly CancellationToken _cancellationToken;

    public GithubCommandProcessor(ICommandContextFactory contextFactory, CancellationToken cancellationToken)
    {
        _contextFactory = contextFactory;
        _cancellationToken = cancellationToken;
    }

    public async Task<BaseShreksCommandResult> VisitAsync(RateCommand rateCommand)
    {
        try
        {
            SubmissionContext context = await _contextFactory.CreateSubmissionContext(_cancellationToken);
            SubmissionDto submission = await rateCommand.ExecuteAsync(context, _cancellationToken);
            return new BaseShreksCommandResult(true, $"Submission rated - {submission.ToPullRequestString()}");
        }
        catch (Exception e)
        {
            return new BaseShreksCommandResult(false, $"Received error while process rate command: {e.ToString()}");
        }
    }

    public async Task<BaseShreksCommandResult> VisitAsync(UpdateCommand updateCommand)
    {
        try
        {
            BaseContext context = await _contextFactory.CreateBaseContext(_cancellationToken);
            SubmissionDto submission = await updateCommand.ExecuteAsync(context, _cancellationToken);
            return new BaseShreksCommandResult(true, $"Submission updated - {submission.ToPullRequestString()}");
        }
        catch (Exception e)
        {
            return new BaseShreksCommandResult(false, $"Received error while process update command: {e.ToString()}");
        }
    }

    public async Task<BaseShreksCommandResult> VisitAsync(HelpCommand helpCommand)
    {
        BaseContext context = await _contextFactory.CreateBaseContext(_cancellationToken);
        string result = await helpCommand.ExecuteAsync(context, _cancellationToken);
        return new BaseShreksCommandResult(true, result);
    }

    public async Task<BaseShreksCommandResult> VisitAsync(ActivateCommand command)
    {
        try
        {
            var context = await _contextFactory.CreateSubmissionContext(_cancellationToken);
            await command.ExecuteAsync(context, _cancellationToken);
            return new BaseShreksCommandResult(true, "Submission activated successfully");
        }
        catch (Exception e)
        {
            return new BaseShreksCommandResult(false, $"Received error while process update command: {e}");
        }
    }

    public async Task<BaseShreksCommandResult> VisitAsync(DeactivateCommand command)
    {
        try
        {
            var context = await _contextFactory.CreateSubmissionContext(_cancellationToken);
            await command.ExecuteAsync(context, _cancellationToken);
            return new BaseShreksCommandResult(true, "Submission deactivated successfully");
        }
        catch (Exception e)
        {
            return new BaseShreksCommandResult(false, $"Received error while process update command: {e}");
        }
    }
}