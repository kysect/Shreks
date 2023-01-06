using CommandLine;
using Kysect.Shreks.Application.Contracts.Submissions.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Commands.CommandVisitors;
using Kysect.Shreks.Commands.Contexts;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Commands.SubmissionCommands;

[Verb("/rate", aliases: new[] { "/assess" })]
public class RateCommand : ISubmissionCommand<SubmissionContext, SubmissionRateDto>
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

    public async Task<SubmissionRateDto> ExecuteAsync(
        SubmissionContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handle /rate command from {IssuerId} with arguments: {Args}",
            context.IssuerId,
            ToLogLine());

        var command = new RateSubmission.Command(context.IssuerId, context.SubmissionId, RatingPercent, ExtraPoints);
        RateSubmission.Response response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }

    public string ToLogLine()
    {
        return $"RatingPercent: {RatingPercent}, ExtraPoints: {ExtraPoints}";
    }
}