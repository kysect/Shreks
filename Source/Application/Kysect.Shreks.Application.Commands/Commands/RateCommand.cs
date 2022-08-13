using CommandLine;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using MediatR;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/rate", aliases: new []{"/assess", "/gachi-rate"})]
public class RateCommand : IShreksCommand
{
    public RateCommand(int ratingPercent, int extraPoints)
    {
        RatingPercent = ratingPercent;
        ExtraPoints = extraPoints;
    }

    [Value(0, Required = true, MetaName = "RatingPercent")]
    public int RatingPercent { get; }
    
    [Value(1, Required = false, Default = 0, MetaName = "ExtraPoints")]
    public int ExtraPoints { get; }
    
    public Task<TResult> Process<TResult>(IShreksCommandProcessor<TResult> processor, ShreksCommandContext context) 
        where TResult : IShreksCommandResult
    {
        return processor.Process(this, context);
    }

    public IEnumerable<IRequest> GetRequest(ShreksCommandContext context)
    {
        Guid submissionId = context.Submission?.Id ?? throw new ArgumentException("No submission provided"); //should it be it's own exception so we can catch it in processor and tell that there is no submission in this pr?
        //TODO: add update extra points command or add extra points to this command
        return new List<IRequest>{new UpdateSubmissionPoints.Command(submissionId, RatingPercent)};
    }
}