using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Extensions;

public static class ShreksCommandProcessorExtensions
{
    public static async Task<BaseShreksCommandResult> ProcessBaseCommand(this IShreksCommandProcessor shreksCommandProcessor,
        IShreksCommand command,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        try
        {
            switch (command)
            {
                case ActivateCommand activateCommand:
                    {
                        await shreksCommandProcessor.Activate(activateCommand, cancellationToken);
                        return new BaseShreksCommandResult(true, "Submission activated successfully");
                    }

                case CreateSubmissionCommand createSubmissionCommand:
                    {
                        SubmissionRateDto submission = await shreksCommandProcessor.CreateSubmission(createSubmissionCommand, cancellationToken);
                        return new BaseShreksCommandResult(true, $"Submission created.\n{submission.ToPullRequestString()}");
                    }

                case DeactivateCommand deactivateCommand:
                    {
                        await shreksCommandProcessor.Deactivate(deactivateCommand, cancellationToken);
                        return new BaseShreksCommandResult(true, "Submission deactivated successfully");
                    }

                case DeleteCommand deleteCommand:
                    {
                        await shreksCommandProcessor.Delete(deleteCommand, cancellationToken);
                        return new BaseShreksCommandResult(true, "Submission deleted successfully");
                    }

                case HelpCommand helpCommand:
                    {
                        string result = await shreksCommandProcessor.Help(helpCommand, cancellationToken);
                        return new BaseShreksCommandResult(true, result);
                    }

                case RateCommand rateCommand:
                    {
                        SubmissionRateDto submission = await shreksCommandProcessor.Rate(rateCommand, cancellationToken);
                        return new BaseShreksCommandResult(true, $"Submission rated.\n{submission.ToPullRequestString()}");
                    }

                case UpdateCommand updateCommand:
                    {
                        SubmissionRateDto submission = await shreksCommandProcessor.Update(updateCommand, cancellationToken);
                        return new BaseShreksCommandResult(true, $"Submission updated.\n{submission.ToPullRequestString()}");
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }
        }
        catch (Exception e)
        {
            string message = $"An error occurred while processing {command.GetType().Name} command: {e.Message}";
            logger.LogError(e, message);
            return new BaseShreksCommandResult(false, message);
        }
    }
}