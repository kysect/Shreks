using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Factories;
using Microsoft.Extensions.Logging;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Application.Commands.Processors;

public class ShreksCommandProcessor
{
    private readonly ICommandContextFactory _commandContextFactory;
    private readonly ILogger _logger;

    public ShreksCommandProcessor(ICommandContextFactory commandContextFactory, ILogger logger)
    {
        _commandContextFactory = commandContextFactory;
        _logger = logger;
    }

    public async Task<BaseShreksCommandResult> ProcessBaseCommandSafe(IShreksCommand command, CancellationToken cancellationToken)
    {
        try
        {
            return await ProcessBaseCommand(command, cancellationToken);
        }
        catch (Exception e)
        {
            var message = $"An error occurred while processing {command.GetType().Name} command: {e.Message}";
            _logger.LogError(e, message);
            return new BaseShreksCommandResult(false, message);
        }
    }

    private async Task<BaseShreksCommandResult> ProcessBaseCommand(IShreksCommand command, CancellationToken cancellationToken)
    {
        switch (command)
        {
            case ActivateCommand activateCommand:
                {
                    SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                    await activateCommand.Execute(context, _logger, cancellationToken);
                    return new BaseShreksCommandResult(true, "Submission activated successfully");
                }

            case CreateSubmissionCommand createSubmissionCommand:
                {
                    PullRequestAndAssignmentContext context = await _commandContextFactory.CreatePullRequestAndAssignmentContext(cancellationToken);
                    SubmissionRateDto submission = await createSubmissionCommand.Execute(context, _logger, cancellationToken);
                    return new BaseShreksCommandResult(true, $"Submission created.\n{submission.ToPullRequestString()}");
                }

            case DeactivateCommand deactivateCommand:
                {
                    SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                    await deactivateCommand.Execute(context, _logger, cancellationToken);
                    return new BaseShreksCommandResult(true, "Submission deactivated successfully");
                }

            case DeleteCommand deleteCommand:
                {
                    SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                    await deleteCommand.Execute(context, _logger, cancellationToken);
                    return new BaseShreksCommandResult(true, "Submission deleted successfully");
                }

            case HelpCommand helpCommand:
                {
                    SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                    string result = helpCommand.Execute(context, _logger);
                    return new BaseShreksCommandResult(true, result);
                }

            case RateCommand rateCommand:
                {
                    SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                    Submission submission = await rateCommand.Execute(context, _logger, cancellationToken);
                    SubmissionRateDto submissionDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);
                    return new BaseShreksCommandResult(true, $"Submission rated.\n{submissionDto.ToPullRequestString()}");
                }

            case UpdateCommand updateCommand:
                {
                    SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                    Submission submission = await updateCommand.Execute(context, _logger, cancellationToken);
                    SubmissionRateDto submissionDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);
                    return new BaseShreksCommandResult(true, $"Submission updated.\n{submissionDto.ToPullRequestString()}");
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(command));
        }
    }
}