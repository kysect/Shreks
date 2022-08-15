using CommandLine;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Core.Study;

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
    public double RatingPercent { get; }
    
    [Value(1, Required = false, Default = 0, MetaName = "ExtraPoints")]
    public double ExtraPoints { get; }
    
    public Task<TResult> Process<TResult>(IShreksCommandProcessor<TResult> processor, ICommandContextCreator contextCreator) 
        where TResult : IShreksCommandResult
    {
        return processor.Process(this, contextCreator);
    }

    public async Task<Guid> Execute(SubmissionContext context)
    {
        Submission submission = context.Submission;
        //TODO: add update extra points command or add extra points to this command
        var respone = await context.Mediator.Send(new UpdateSubmissionPoints.Command(submission.Id, 
            RatingPercent / 100 * submission.Assignment.MaxPoints));
        return respone.Submission.Id;
    }
}