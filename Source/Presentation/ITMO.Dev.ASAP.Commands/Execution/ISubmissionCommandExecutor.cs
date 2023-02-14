using ITMO.Dev.ASAP.Commands.Result;
using ITMO.Dev.ASAP.Commands.SubmissionCommands;

namespace ITMO.Dev.ASAP.Commands.Execution;

public interface ISubmissionCommandExecutor
{
    Task<ICommandResult> ExecuteAsync(ISubmissionCommand command, CancellationToken cancellationToken);
}