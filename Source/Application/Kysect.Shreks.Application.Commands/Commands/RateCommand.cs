using CommandLine;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;

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
    
    public Task<TResult> Process<TResult>(IShreksCommandProcessor<TResult> processor, ICommandContextFactory contextFactory) 
        where TResult : IShreksCommandResult
    {
        return processor.Process(this, contextFactory);
    }

    public async Task<Guid> Execute(SubmissionContext context)
    {
        var submissionId = context.Submission.Id;
        var maxPoints = context.Submission.Assignment.MaxPoints;
        var points = RatingPercent / 100 * maxPoints;
        var command = new UpdateSubmissionPoints.Command(submissionId, points);
        //TODO: add update extra points command or add extra points to this command
        var respone = await context.Mediator.Send(command);
        return respone.Submission.Id;
    }
}