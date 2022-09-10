using CommandLine;

namespace Kysect.Shreks.Application.UserCommands.Abstractions.Commands;

[Verb("/update")]
public class UpdateCommand : IShreksCommand
{
    public UpdateCommand(int submissionCode, double? ratingPercent, double? extraPoints, string? dateStr)
    {
        SubmissionCode = submissionCode;
        RatingPercent = ratingPercent;
        ExtraPoints = extraPoints;
        DateStr = dateStr;
    }

    [Value(0, Required = true, MetaName = "SubmissionCode")]
    public int SubmissionCode { get; }
    
    [Option(shortName:'r', longName: "rating", Group = "update",  Required = false)]
    public double? RatingPercent { get; }
    
    [Option(shortName:'e', longName:"extra", Group = "update", Required = false)]
    public double? ExtraPoints { get; }
    
    [Option(shortName:'d', longName:"date", Group = "update", Required = false)]
    public string? DateStr { get; }

    public string ToLogLine()
    {
        return $" {{ SubmissionCode : {SubmissionCode}," +
               $" RatingPercent: {RatingPercent}" +
               $" ExtraPoints: {ExtraPoints}" +
               $" DateStr: {DateStr}" +
               $" }}";
    }
}