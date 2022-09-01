using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Integration.Github.Processors;

public class GithubCommandProcessor : IShreksCommandVisitor<BaseShreksCommandResult>
{
    private readonly ICommandContextFactory _contextFactory;
    private readonly CancellationToken _cancellationToken;
    private readonly ILogger _logger;

    public GithubCommandProcessor(ICommandContextFactory contextFactory, ILogger logger, CancellationToken cancellationToken)
    {
        _contextFactory = contextFactory;
        _cancellationToken = cancellationToken;
        _logger = logger;
    }

    public async Task<BaseShreksCommandResult> VisitAsync(RateCommand rateCommand)
    {
        try
        {
            SubmissionContext context = await _contextFactory.CreateSubmissionContext(_cancellationToken);
            SubmissionRateDto submission = await rateCommand.ExecuteAsync(context, _cancellationToken);
            return new BaseShreksCommandResult(true, $"Submission rated - {submission.ToPullRequestString()}");
        }
        catch (ShreksDomainException e)
        {
            string message = $"An error occurred while processing rate command: {e.Message}";
            _logger.LogError(e, message);
            return new BaseShreksCommandResult(false, message);
        }
    }

    public async Task<BaseShreksCommandResult> VisitAsync(UpdateCommand updateCommand)
    {
        try
        {
            PullRequestContext context = await _contextFactory.CreatePullRequestContext(_cancellationToken);
            SubmissionRateDto submission = await updateCommand.ExecuteAsync(context, _cancellationToken);
            return new BaseShreksCommandResult(true, $"Submission updated - {submission.ToPullRequestString()}");
        }
        catch (ShreksDomainException e)
        {
            string message = $"An error occurred while processing update command: {e.Message}";
            _logger.LogError(e, message);
            return new BaseShreksCommandResult(false, message);
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
        catch (ShreksDomainException e)
        {
            string message = $"An error occurred while processing activate command: {e.Message}";
            _logger.LogError(e, message);
            return new BaseShreksCommandResult(false, message);
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
        catch (ShreksDomainException e)
        {
            string message = $"An error occurred while processing deactivate command: {e.Message}";
            _logger.LogError(e, message);
            return new BaseShreksCommandResult(false, message);
        }
    }

    public async Task<BaseShreksCommandResult> VisitAsync(CreateSubmissionCommand command)
    {
        try
        {
            var context = await _contextFactory.CreatePullRequestAndAssignmentContext(_cancellationToken);
            SubmissionRateDto submission = await command.ExecuteAsync(context, _cancellationToken);
            return new BaseShreksCommandResult(true, $"Submission created - {submission.ToPullRequestString()}");

        }
        catch (ShreksDomainException e)
        {
            string message = $"An error occurred while processing deactivate command: {e.Message}";
            _logger.LogError(e, message);
            return new BaseShreksCommandResult(false, message);
        }
    }

    public async Task<BaseShreksCommandResult> VisitAsync(DeleteCommand command)
    {
        try
        {
            var context = await _contextFactory.CreateSubmissionContext(_cancellationToken);
            await command.ExecuteAsync(context, _cancellationToken);
            return new BaseShreksCommandResult(true, "Submission deleted successfully");
        }
        catch (ShreksDomainException e)
        {
            string message = $"An error occurred while processing delete command: {e.Message}";
            _logger.LogError(e, message);
            return new BaseShreksCommandResult(false, message);
        }
    }
}