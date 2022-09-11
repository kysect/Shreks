using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

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

    public async Task<Submission> ExecuteAsync(SubmissionContext context, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle /update command from {context.IssuerId} with arguments: {ToLogLine()}");

        Submission submission = null!;

        if (RatingPercent is not null || ExtraPoints is not null)
        {
            submission = await context.SubmissionService.UpdateSubmissionPoints(
                context.SubmissionId,
                context.IssuerId,
                RatingPercent,
                ExtraPoints,
                cancellationToken);

            submission = await context.SubmissionService.UpdateSubmissionState(
                context.SubmissionId,
                context.IssuerId,
                SubmissionState.Completed,
                cancellationToken);
        }

        if (DateStr is not null)
        {
            submission = await context.SubmissionService.UpdateSubmissionDate(
                context.SubmissionId,
                context.IssuerId,
                GetDate(),
                cancellationToken);
        }

        return submission;
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