using CommandLine;
using Kysect.Shreks.Common.Exceptions;

namespace Kysect.Shreks.Application.Commands.Commands;

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

    public DateOnly GetDate()
    {
        if (!DateOnly.TryParse(DateStr, out DateOnly date))
            throw new InvalidUserInputException($"Cannot parse input date ({DateStr} as date. Ensure that you use correct format.");

        return date;
    }
    
    public string ToLogLine()
    {
        return $" {{ SubmissionCode : {SubmissionCode}," +
               $" RatingPercent: {RatingPercent}" +
               $" ExtraPoints: {ExtraPoints}" +
               $" DateStr: {DateStr}" +
               $" }}";
    }
}