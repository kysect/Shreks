using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Application.Commands.Commands;

public interface IShreksCommand
{
    Task<TResult> AcceptAsync<TResult>(IShreksCommandVisitor<TResult> visitor) where TResult : IShreksCommandResult;
}

public interface IShreksCommand<TContext, TResult> : IShreksCommand
{
    Task<TResult> ExecuteAsync(TContext context, CancellationToken cancellationToken);
}