using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Application.Commands.Processors;

public interface IShreksCommandProcessor<TResult> where TResult : IShreksCommandResult
{
    Task<TResult> Process(RateCommand rateCommand, ICommandContextCreator contextCreator);
    Task<TResult> Process(UpdateCommand updateCommand, ICommandContextCreator contextCreator);
}