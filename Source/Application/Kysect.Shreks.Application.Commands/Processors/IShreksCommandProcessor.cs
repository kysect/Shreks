using System.Reflection.Metadata;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Processors;

public interface IShreksCommandProcessor<TResult> where TResult : IShreksCommandResult
{
    Task<TResult> Process(RateCommand rateCommand, ShreksCommandContext context);
    Task<TResult> Process(UpdateCommand updateCommand, ShreksCommandContext context);
}