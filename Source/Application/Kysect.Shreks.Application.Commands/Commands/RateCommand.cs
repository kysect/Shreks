using CommandLine;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/rate", aliases: new []{"/assess", "/gachi-rate"})]
public class RateCommand : IShreksCommand
{
    public RateCommand(int ratingPercent, int extraPoints)
    {
        RatingPercent = ratingPercent;
        ExtraPoints = extraPoints;
    }

    [Value(0, Required = true, Min = 0, Max = 100, MetaName = "RatingPercent")]
    private int RatingPercent { get; }
    
    [Value(1, Required = false, Default = 0, MetaName = "ExtraPoints")]
    private int ExtraPoints { get; }
    
    public Task<TResult> Process<TResult>(IShreksCommandProcessor<TResult> processor, User executor) where TResult : IShreksCommandResult
    {
        return processor.Process(this, executor);
    }
}