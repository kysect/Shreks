using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Application.Commands.Commands;

public interface IShreksCommand
{
    Task<TResult> Accept<TResult>(IShreksCommandVisitor<TResult> visitor, CancellationToken cancellationToken)
        where TResult : IShreksCommandResult;
}