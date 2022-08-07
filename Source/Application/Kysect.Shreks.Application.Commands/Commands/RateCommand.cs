using CommandLine;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/rate", aliases: new []{"/assess", "/gachi-rate"})]
public class RateCommand : ICommand
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
    
    public void Process(/*mediatr*/)
    {
        //create and submit request
    }
}