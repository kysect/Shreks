using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.UserCommands.Abstractions;
using Kysect.Shreks.Application.UserCommands.Abstractions.Commands;
using Kysect.Shreks.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow;

public class GithubCommandProcessor : IShreksCommandVisitor<BaseShreksCommandResult>
{
    private readonly ICommandContextFactory _contextFactory;
    private readonly CancellationToken _cancellationToken;
    private readonly ILogger _logger;
    private readonly IShreksCommandProcessor _shreksCommandProcessor;

    public GithubCommandProcessor(ICommandContextFactory contextFactory, ILogger logger, CancellationToken cancellationToken, IShreksCommandProcessor shreksCommandProcessor)
    {
        _contextFactory = contextFactory;
        _cancellationToken = cancellationToken;
        _shreksCommandProcessor = shreksCommandProcessor;
        _logger = logger;
    }

    public async Task<BaseShreksCommandResult> VisitAsync(RateCommand rateCommand)
    {
        try
        {
            SubmissionRateDto submission = await _shreksCommandProcessor.Rate(rateCommand, _cancellationToken);
            return new BaseShreksCommandResult(true, $"Submission rated.\n{submission.ToPullRequestString()}");
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
            SubmissionRateDto submission = await _shreksCommandProcessor.Update(updateCommand, _cancellationToken);
            return new BaseShreksCommandResult(true, $"Submission updated.\n{submission.ToPullRequestString()}");
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
        string result = await _shreksCommandProcessor.Help(helpCommand, _cancellationToken);
        return new BaseShreksCommandResult(true, result);
    }

    public async Task<BaseShreksCommandResult> VisitAsync(ActivateCommand command)
    {
        try
        {
            await _shreksCommandProcessor.Activate(command, _cancellationToken);
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
            await _shreksCommandProcessor.Deactivate(command, _cancellationToken);
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
            SubmissionRateDto submission = await _shreksCommandProcessor.CreateSubmission(command, _cancellationToken);
            return new BaseShreksCommandResult(true, $"Submission created.\n{submission.ToPullRequestString()}");
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
            await _shreksCommandProcessor.Delete(command, _cancellationToken);
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