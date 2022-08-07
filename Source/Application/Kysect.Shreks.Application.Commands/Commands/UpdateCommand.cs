using CommandLine;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/update")]
public class UpdateCommand : ICommand
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
    public void Process(ICommandProcessor processor, User executor)
    {
        processor.Process(this, executor);
    }
}