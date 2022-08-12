using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Commands;

public interface IShreksCommand
{
    public Task<TResult> Process<TResult>(IShreksCommandProcessor<TResult> processor, User executor)
        where TResult : IShreksCommandResult;
}