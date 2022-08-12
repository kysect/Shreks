using CommandLine;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/update")]
public class UpdateCommand : IShreksCommand
{
    public UpdateCommand(string submissionId, int ratingPercent, int extraPoints)
    {
        SubmissionId = submissionId;
        RatingPercent = ratingPercent;
        ExtraPoints = extraPoints;
    }

    [Value(0, Required = true, MetaName = "SubmissionId")]
    private string SubmissionId { get; }
    
    [Option(shortName:'r', longName: "rating", Required = false, Min = 0, Max = 100)]
    private int? RatingPercent { get; }
    
    [Option(shortName:'e', longName:"extra", Required = false)]
    private int? ExtraPoints { get; }
    
    public Task<TResult> Process<TResult>(IShreksCommandProcessor<TResult> processor, User executor) where TResult : IShreksCommandResult
    {
        return processor.Process(this, executor);
    }
}