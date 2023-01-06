using Kysect.Shreks.Commands.CommandVisitors;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Commands.SubmissionCommands;

public interface ISubmissionCommand
{
    Task AcceptAsync(ISubmissionCommandVisitor visitor);
}

public interface ISubmissionCommand<in TContext, TResult> : ISubmissionCommand
{
    Task<TResult> ExecuteAsync(TContext context, ILogger logger, CancellationToken cancellationToken);
}