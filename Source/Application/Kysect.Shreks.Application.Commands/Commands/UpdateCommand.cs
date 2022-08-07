using CommandLine;

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
    
    public void Process(/*mediatr*/)
    {
        //create and submit request
    }
}