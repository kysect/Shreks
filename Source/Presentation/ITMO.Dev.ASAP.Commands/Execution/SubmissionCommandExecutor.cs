using ITMO.Dev.ASAP.Commands.CommandVisitors;
using ITMO.Dev.ASAP.Commands.Contexts;
using ITMO.Dev.ASAP.Commands.Result;
using ITMO.Dev.ASAP.Commands.SubmissionCommands;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Common.Resources;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Commands.Execution;

public class SubmissionCommandExecutor : ISubmissionCommandExecutor
{
    private readonly ISubmissionCommandContextFactory _contextFactory;
    private readonly ILogger _logger;

    public SubmissionCommandExecutor(ISubmissionCommandContextFactory contextFactory, ILogger logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<ICommandResult> ExecuteAsync(
        ISubmissionCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var visitor = new SubmissionCommandVisitor(_contextFactory, _logger, cancellationToken);
            await command.AcceptAsync(visitor);

            ICommandResult? result = visitor.Result;

            return result ?? BaseCommandResult.Fail("Failed to execute command");
        }
        catch (DomainException e)
        {
            string commandName = command.GetType().Name;
            string title = UserCommandProcessingMessage.DomainExceptionWhileProcessingUserCommand(commandName);
            string message = $"{title} {e.Message}";

            _logger.LogError(e, "{Title}: {Message}", title, message);
            return BaseCommandResult.Fail(message);
        }
        catch (Exception e)
        {
            string message = UserCommandProcessingMessage.InternalExceptionWhileProcessingUserCommand();

            _logger.LogError(e, "{Message}", message);
            return BaseCommandResult.Fail(message);
        }
    }
}