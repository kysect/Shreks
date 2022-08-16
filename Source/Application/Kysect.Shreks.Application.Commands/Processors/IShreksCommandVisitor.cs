using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Application.Commands.Processors;

public interface IShreksCommandVisitor<TResult> where TResult : IShreksCommandResult
{
    Task<TResult> Visit(RateCommand rateCommand, ICommandContextFactory contextFactory, 
        CancellationToken cancellationToken);
    Task<TResult> Visit(UpdateCommand updateCommand, ICommandContextFactory contextFactory,
        CancellationToken cancellationToken);
}