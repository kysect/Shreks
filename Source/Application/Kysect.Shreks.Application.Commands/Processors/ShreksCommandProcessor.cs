using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Factories;
using Microsoft.Extensions.Logging;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Models;

namespace Kysect.Shreks.Application.Commands.Processors;

public class ShreksCommandProcessor : IShreksCommandProcessor
{
    private readonly ICommandContextFactory _commandContextFactory;
    private readonly IShreksCommandService _shreksCommandService;

    public ShreksCommandProcessor(ICommandContextFactory commandContextFactory, IShreksCommandService shreksCommandService)
    {
        _commandContextFactory = commandContextFactory;
        _shreksCommandService = shreksCommandService;
    }

    public async Task<SubmissionRateDto> Rate(RateCommand rateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        string message = $"Handle /rate command from {context.IssuerId} with arguments:" +
                         $" {{ RatingPercent: {rateCommand.RatingPercent}," +
                         $" ExtraPoints: {rateCommand.ExtraPoints}}}";
        context.Log.LogInformation(message);

        Submission submission = await _shreksCommandService.UpdateSubmissionPoints(
            context.Submission.Id,
            context.IssuerId,
            rateCommand.RatingPercent,
            rateCommand.ExtraPoints,
            cancellationToken);
        return SubmissionRateDtoFactory.CreateFromSubmission(submission);
    }

    public async Task<SubmissionRateDto> Update(UpdateCommand updateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        string message = $"Handle /update command from {context.IssuerId} with arguments:" + updateCommand.ToLogLine();
        context.Log.LogInformation(message);

        SubmissionRateDto submissionRateDto = null!;

        var submissionResponse = context.Submission;

        if (updateCommand.RatingPercent is not null || updateCommand.ExtraPoints is not null)
        {
            context.Log.LogInformation($"Invoke update command for submission {submissionResponse.Id} with arguments:" +
                                       $"{{ Rating: {updateCommand.RatingPercent}," +
                                       $" ExtraPoints: {updateCommand.ExtraPoints}}}");

            Submission submission = await _shreksCommandService.UpdateSubmissionPoints(
                context.Submission.Id,
                context.IssuerId,
                updateCommand.RatingPercent,
                updateCommand.ExtraPoints,
                cancellationToken);
            submissionRateDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);
        }

        if (updateCommand.DateStr is not null)
        {
            if (!DateOnly.TryParse(updateCommand.DateStr, out DateOnly date))
                throw new InvalidUserInputException($"Cannot parse input date ({updateCommand.DateStr} as date. Ensure that you use correct format.");

            Submission submission = await _shreksCommandService.UpdateSubmissionDate(context.Submission.Id, context.IssuerId, date, cancellationToken);
            submissionRateDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);
        }

        return submissionRateDto;
    }

    public async Task<string> Help(HelpCommand helpCommand, CancellationToken token)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(token);

        context.Log.LogDebug($"Handle /help command from {context.IssuerId}");

        return HelpCommand.HelpString;
    }

    public async Task<Submission> Activate(ActivateCommand activateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        context.Log.LogInformation($"Handle /activate command for submission {context.Submission.Id} from user {context.IssuerId}");
        Submission submission = await _shreksCommandService.UpdateSubmissionState(context.Submission.Id, context.IssuerId, SubmissionState.Active, cancellationToken);
        return submission;
    }

    public async Task<Submission> Deactivate(DeactivateCommand deactivateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        context.Log.LogInformation($"Handle /deactivate command for submission {context.Submission.Id} from user {context.IssuerId}");
        Submission submission = await _shreksCommandService.UpdateSubmissionState(context.Submission.Id, context.IssuerId, SubmissionState.Inactive, cancellationToken);
        return submission;
    }

    public async Task<SubmissionRateDto> CreateSubmission(CreateSubmissionCommand createSubmissionCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreatePullRequestAndAssignmentContext(cancellationToken);

        context.Log.LogInformation($"Handle /create-submission command for pr {context.PullRequestDescriptor.Payload}");

        SubmissionRateDto result = await context.CommandSubmissionFactory.CreateSubmission(
            context.IssuerId,
            context.AssignmentId,
            context.PullRequestDescriptor);

        return result;
    }

    public async Task<Submission> Delete(DeleteCommand deleteCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
        context.Log.LogInformation($"Handle /delete command for submission {context.Submission.Id} from user {context.IssuerId}");

        Submission submission = await _shreksCommandService.UpdateSubmissionState(context.Submission.Id, context.IssuerId, SubmissionState.Deleted, cancellationToken);
        return submission;
    }
}