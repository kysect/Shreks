using CommandLine;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/update")]
public class UpdateCommand : IShreksCommand<SubmissionContext, SubmissionRateDto>
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
    
    public Task<TResult> AcceptAsync<TResult>(IShreksCommandVisitor<TResult> visitor) 
        where TResult : IShreksCommandResult
    {
        return visitor.VisitAsync(this);
    }

    public async Task<SubmissionRateDto> ExecuteAsync(SubmissionContext context, CancellationToken cancellationToken)
    {
        string message = $"Handle /update command from {context.IssuerId} with arguments:" +
                         $" {{ SubmissionCode : {SubmissionCode}," +
                         $" RatingPercent: {RatingPercent}" +
                         $" ExtraPoints: {ExtraPoints}" +
                         $" DateStr: {DateStr}" +
                         $" }}";
        context.Log.LogInformation(message);

        SubmissionRateDto submissionRateDto = null!;

        var submissionResponse = context.Submission;

        if (RatingPercent is not null || ExtraPoints is not null)
        {
            context.Log.LogInformation($"Invoke update command for submission {submissionResponse.Id} with arguments:" +
                                       $"{{ Rating: {RatingPercent}," +
                                       $" ExtraPoints: {ExtraPoints}}}");

            var command = new UpdateSubmissionPoints.Command(submissionResponse.Id, context.IssuerId, RatingPercent, ExtraPoints);
            var response = await context.Mediator.Send(command, cancellationToken);
            submissionRateDto = response.SubmissionRate;
        }

        if (DateStr is not null)
        {
            if (!DateOnly.TryParse(DateStr, out DateOnly date))
                throw new InvalidUserInputException($"Cannot parse input date ({DateStr} as date. Ensure that you use correct format.");

            var command = new UpdateSubmissionDate.Command(submissionResponse.Id, context.IssuerId, date.ToDateTime(TimeOnly.MinValue));
            var response = await context.Mediator.Send(command, cancellationToken);
            submissionRateDto = response.SubmissionRate;
        }

        return submissionRateDto;
    }
}