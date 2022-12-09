using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;
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

    [Option(shortName: 'r', longName: "rating", Group = "update", Required = false)]
    public double? RatingPercent { get; }

    [Option(shortName: 'e', longName: "extra", Group = "update", Required = false)]
    public double? ExtraPoints { get; }

    [Option(shortName: 'd', longName: "date", Group = "update", Required = false)]
    public string? DateStr { get; }

    public DateOnly GetDate()
    {
        return RuCultureDate.Parse(DateStr);
    }

    public async Task<Submission> ExecuteAsync(UpdateContext context, ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle /update command from {context.IssuerId} with arguments: {ToLogLine()}");

        Submission submission = await context.SubmissionService.GetSubmissionByCodeAsync(
            SubmissionCode,
            context.Student.Id,
            context.Assignment.Id,
            cancellationToken);

        if (RatingPercent is not null || ExtraPoints is not null)
        {
            submission = await context.SubmissionService.UpdateSubmissionPoints(
                submission.Id,
                context.IssuerId,
                RatingPercent,
                ExtraPoints,
                cancellationToken);
        }

        if (DateStr is not null)
        {
            submission = await context.SubmissionService.UpdateSubmissionDate(
                submission.Id,
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