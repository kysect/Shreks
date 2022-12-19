using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

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

    public async Task<BaseShreksCommandResult> ProcessBaseCommandSafe(
        IShreksCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ProcessBaseCommand(command, cancellationToken);
        }
        catch (ShreksDomainException e)
        {
            string commandName = command.GetType().Name;
            string title = UserCommandProcessingMessage.DomainExceptionWhileProcessingUserCommand(commandName);
            string message = $"{title} {e.Message}";

            _logger.LogError(e, message);
            return BaseShreksCommandResult.Fail(message);
        }
        catch (Exception e)
        {
            string message = UserCommandProcessingMessage.InternalExceptionWhileProcessingUserCommand();
            _logger.LogError(e, message);
            return BaseShreksCommandResult.Fail(message);
        }
    }

    private async Task<BaseShreksCommandResult> ProcessBaseCommand(
        IShreksCommand command,
        CancellationToken cancellationToken)
    {
        switch (command)
        {
            case ActivateCommand activateCommand:
            {
                SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                await activateCommand.ExecuteAsync(context, _logger, cancellationToken);
                return BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionActivatedSuccessfully());
            }

            case CreateSubmissionCommand createSubmissionCommand:
            {
                PayloadAndAssignmentContext context = await _commandContextFactory
                    .CreatePayloadAndAssignmentContext(cancellationToken);

                SubmissionRateDto submission = await createSubmissionCommand
                    .ExecuteAsync(context, _logger, cancellationToken);

                string message = UserCommandProcessingMessage.SubmissionCreated(submission.ToDisplayString());

                return BaseShreksCommandResult.Success(message);
            }

            case DeactivateCommand deactivateCommand:
            {
                SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                await deactivateCommand.ExecuteAsync(context, _logger, cancellationToken);
                return BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionDeactivatedSuccessfully());
            }

            case DeleteCommand deleteCommand:
            {
                SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                await deleteCommand.ExecuteAsync(context, _logger, cancellationToken);
                return BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionDeletedSuccessfully());
            }

            case HelpCommand helpCommand:
            {
                BaseContext context = await _commandContextFactory.CreateBaseContext(cancellationToken);
                string result = helpCommand.ExecuteAsync(context, _logger);
                return BaseShreksCommandResult.Success(result);
            }

            case RateCommand rateCommand:
            {
                SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                Submission submission = await rateCommand.ExecuteAsync(context, _logger, cancellationToken);
                SubmissionRateDto submissionDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

                string message = UserCommandProcessingMessage.SubmissionRated(submissionDto.ToDisplayString());

                return BaseShreksCommandResult.Success(message);
            }

            case UpdateCommand updateCommand:
            {
                UpdateContext context = await _commandContextFactory.CreateUpdateContextAsync(cancellationToken);
                Submission submission = await updateCommand.ExecuteAsync(context, _logger, cancellationToken);
                SubmissionRateDto submissionDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

                string message = UserCommandProcessingMessage.SubmissionUpdated(submissionDto.ToDisplayString());

                return BaseShreksCommandResult.Success(message);
            }

            case MarkReviewedCommand markReviewedCommand:
            {
                SubmissionContext context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
                await markReviewedCommand.ExecuteAsync(context, cancellationToken);
                return BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionMarkedAsReviewed());
            }

            case BanCommand banCommand:
            {
                UpdateContext context = await _commandContextFactory.CreateUpdateContextAsync(cancellationToken);
                Submission submission = await banCommand.ExecuteAsync(context, _logger, cancellationToken);
                return BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionBanned(submission.Id));
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(command));
        }
    }
}