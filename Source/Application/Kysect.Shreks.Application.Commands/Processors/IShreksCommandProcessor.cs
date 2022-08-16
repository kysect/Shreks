using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Application.Commands.Processors;

public interface IShreksCommandProcessor<TResult> where TResult : IShreksCommandResult
{
    Task<TResult> Process(RateCommand rateCommand, ICommandContextFactory contextFactory);
    Task<TResult> Process(UpdateCommand updateCommand, ICommandContextFactory contextFactory);
}