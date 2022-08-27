using CommandLine;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/rate", aliases: new []{"/assess"})]
public class RateCommand : IShreksCommand<SubmissionContext, SubmissionRateDto>
{
    public RateCommand(double ratingPercent, double? extraPoints)
    {
        RatingPercent = ratingPercent;
        ExtraPoints = extraPoints;
    }

    [Value(0, Required = true, MetaName = "RatingPercent")]
    public double RatingPercent { get; }
    
    [Value(1, Required = false, Default = 0.0, MetaName = "ExtraPoints")]
    public double? ExtraPoints { get; }
    
    public Task<TResult> AcceptAsync<TResult>(IShreksCommandVisitor<TResult> visitor) 
        where TResult : IShreksCommandResult
    {
        return visitor.VisitAsync(this);
    }

    public async Task<SubmissionRateDto> ExecuteAsync(SubmissionContext context, CancellationToken cancellationToken)
    {
        string message = $"Handle /rate command from {context.IssuerId} with arguments:" +
                         $" {{ RatingPercent: {RatingPercent}," +
                         $" ExtraPoints: {ExtraPoints}}}";
        context.Log.LogInformation(message);

        var submissionId = context.Submission.Id;
        var command = new UpdateSubmissionPoints.Command(submissionId, RatingPercent, ExtraPoints);
        var response = await context.Mediator.Send(command, cancellationToken);
        return response.SubmissionRate;
    }
}