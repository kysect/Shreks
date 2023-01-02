using Kysect.Shreks.Commands.Result;
using Kysect.Shreks.Commands.SubmissionCommands;

namespace Kysect.Shreks.Commands.Execution;

public interface ISubmissionCommandExecutor
{
    Task<IShreksCommandResult> ExecuteAsync(ISubmissionCommand command, CancellationToken cancellationToken);
}