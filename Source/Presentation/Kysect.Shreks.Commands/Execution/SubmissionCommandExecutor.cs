using Kysect.Shreks.Commands.CommandVisitors;
using Kysect.Shreks.Commands.Contexts;
using Kysect.Shreks.Commands.Result;
using Kysect.Shreks.Commands.SubmissionCommands;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Common.Resources;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Commands.Execution;

public class SubmissionCommandExecutor : ISubmissionCommandExecutor
{
    private readonly ISubmissionCommandContextFactory _contextFactory;
    private readonly ILogger _logger;

    public SubmissionCommandExecutor(ISubmissionCommandContextFactory contextFactory, ILogger logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<IShreksCommandResult> ExecuteAsync(ISubmissionCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var visitor = new SubmissionCommandVisitor(_contextFactory, _logger, cancellationToken);
            await command.AcceptAsync(visitor);

            IShreksCommandResult? result = visitor.Result;

            return result ?? BaseShreksCommandResult.Fail("Failed to execute command");
        }
        catch (ShreksDomainException e)
        {
            string commandName = command.GetType().Name;
            string title = UserCommandProcessingMessage.DomainExceptionWhileProcessingUserCommand(commandName);
            string message = $"{title} {e.Message}";

            _logger.LogError(e, "{Title}: {Message}", title, message);
            return BaseShreksCommandResult.Fail(message);
        }
        catch (Exception e)
        {
            string message = UserCommandProcessingMessage.InternalExceptionWhileProcessingUserCommand();

            _logger.LogError(e, "{Message}", message);
            return BaseShreksCommandResult.Fail(message);
        }
    }
}