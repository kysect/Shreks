using CommandLine;
using Kysect.Shreks.Application.Contracts.Submissions.Commands;
using Kysect.Shreks.Application.Contracts.Submissions.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Commands.CommandVisitors;
using Kysect.Shreks.Commands.Contexts;
using Kysect.Shreks.Common.Tools;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Commands.SubmissionCommands;

[Verb("/update")]
public class UpdateCommand : ISubmissionCommand<UpdateContext, SubmissionRateDto>
{
    public UpdateCommand(int? submissionCode, double? ratingPercent, double? extraPoints, string? dateStr)
    {
        SubmissionCode = submissionCode;
        RatingPercent = ratingPercent;
        ExtraPoints = extraPoints;
        DateStr = dateStr;
    }

    [Value(0, Required = false, MetaName = "SubmissionCode")]
    public int? SubmissionCode { get; }

    [Option('r', "rating", Group = "update", Required = false)]
    public double? RatingPercent { get; }

    [Option('e', "extra", Group = "update", Required = false)]
    public double? ExtraPoints { get; }

    [Option('d', "date", Group = "update", Required = false)]
    public string? DateStr { get; }

    public async Task<SubmissionRateDto> ExecuteAsync(
        UpdateContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handle /update command from {IssuerId} with arguments: {Args}",
            context.IssuerId,
            ToLogLine());

        SubmissionDto submission = SubmissionCode is null
            ? await context.GetDefaultSubmissionAsync(cancellationToken)
            : await GetSubmissionByCodeAsync(context, SubmissionCode.Value, cancellationToken);

        SubmissionRateDto? rateDto = null;

        if (RatingPercent is not null || ExtraPoints is not null)
        {
            var command = new UpdateSubmissionPoints.Command(
                context.IssuerId, submission.Id, RatingPercent, ExtraPoints);

            UpdateSubmissionPoints.Response response = await context.Mediator.Send(command, cancellationToken);

            rateDto = response.Submission;
        }

        if (DateStr is not null)
        {
            var command = new UpdateSubmissionDate.Command(context.IssuerId, submission.Id, GetDate());
            UpdateSubmissionDate.Response response = await context.Mediator.Send(command, cancellationToken);

            rateDto = response.Submission;
        }

        return rateDto ?? throw new InvalidOperationException("No update command was executed");
    }

    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }

    public DateOnly GetDate()
    {
        return RuCultureDate.Parse(DateStr);
    }

    public string ToLogLine()
    {
        return $" {{ SubmissionCode : {SubmissionCode}," +
               $" RatingPercent: {RatingPercent}" +
               $" ExtraPoints: {ExtraPoints}" +
               $" DateStr: {DateStr}" +
               " }";
    }

    private async Task<SubmissionDto> GetSubmissionByCodeAsync(
        UpdateContext context,
        int code,
        CancellationToken cancellationToken)
    {
        var query = new GetSubmissionByCode.Query(context.Student.Id, context.Assignment.Id, code);
        GetSubmissionByCode.Response response = await context.Mediator.Send(query, cancellationToken);

        return response.Submission;
    }
}