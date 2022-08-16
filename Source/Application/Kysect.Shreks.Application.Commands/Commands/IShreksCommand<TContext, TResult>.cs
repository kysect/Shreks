namespace Kysect.Shreks.Application.Commands.Commands;

public interface IShreksCommand<TContext, TResult> : IShreksCommand
{
    Task<TResult> ExecuteAsync(TContext context, CancellationToken cancellationToken);
}