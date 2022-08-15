using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Application.Commands.Commands;

public interface IShreksCommand
{
    public Task<TResult> Process<TResult>(IShreksCommandProcessor<TResult> processor, ICommandContextCreator contextCreator)
        where TResult : IShreksCommandResult;
}