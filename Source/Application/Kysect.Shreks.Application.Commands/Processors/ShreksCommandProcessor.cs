using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Factories;
using Microsoft.Extensions.Logging;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Models;

namespace Kysect.Shreks.Application.Commands.Processors;

public class ShreksCommandProcessor : IShreksCommandProcessor
{
    private readonly ICommandContextFactory _commandContextFactory;
    private readonly ISubmissionService _submissionService;
    private readonly ILogger _logger;

    public ShreksCommandProcessor(ICommandContextFactory commandContextFactory, ISubmissionService submissionService, ILogger logger)
    {
        _commandContextFactory = commandContextFactory;
        _submissionService = submissionService;
        _logger = logger;
    }

    public async Task<SubmissionRateDto> Rate(RateCommand rateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        _logger.LogInformation($"Handle /rate command from {context.IssuerId} with arguments: {rateCommand.ToLogLine()}");

        Submission submission = await _submissionService.UpdateSubmissionPoints(
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

        _logger.LogInformation($"Handle /update command from {context.IssuerId} with arguments: {updateCommand.ToLogLine()}");

        Submission submission = null!;

        if (updateCommand.RatingPercent is not null || updateCommand.ExtraPoints is not null)
        {
            submission = await _submissionService.UpdateSubmissionPoints(
                context.Submission.Id,
                context.IssuerId,
                updateCommand.RatingPercent,
                updateCommand.ExtraPoints,
                cancellationToken);
        }

        if (updateCommand.DateStr is not null)
        {
            submission = await _submissionService.UpdateSubmissionDate(
                context.Submission.Id,
                context.IssuerId,
                updateCommand.GetDate(),
                cancellationToken);
        }

        return SubmissionRateDtoFactory.CreateFromSubmission(submission);
    }

    public async Task<string> Help(HelpCommand helpCommand, CancellationToken token)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(token);

        _logger.LogDebug($"Handle /help command from {context.IssuerId}");

        return HelpCommand.HelpString;
    }

    public async Task<Submission> Activate(ActivateCommand activateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        _logger.LogInformation($"Handle /activate command for submission {context.Submission.Id} from user {context.IssuerId}");
        Submission submission = await _submissionService.UpdateSubmissionState(context.Submission.Id, context.IssuerId, SubmissionState.Active, cancellationToken);
        return submission;
    }

    public async Task<Submission> Deactivate(DeactivateCommand deactivateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        _logger.LogInformation($"Handle /deactivate command for submission {context.Submission.Id} from user {context.IssuerId}");
        Submission submission = await _submissionService.UpdateSubmissionState(context.Submission.Id, context.IssuerId, SubmissionState.Inactive, cancellationToken);
        return submission;
    }

    public async Task<SubmissionRateDto> CreateSubmission(CreateSubmissionCommand createSubmissionCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreatePullRequestAndAssignmentContext(cancellationToken);

        _logger.LogInformation($"Handle /create-submission command for pr {context.PullRequestDescriptor.Payload}");

        return await context.CommandSubmissionFactory.CreateSubmission(
            context.IssuerId,
            context.AssignmentId,
            context.PullRequestDescriptor);
    }

    public async Task<Submission> Delete(DeleteCommand deleteCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);
        
        _logger.LogInformation($"Handle /delete command for submission {context.Submission.Id} from user {context.IssuerId}");
        
        Submission submission = await _submissionService.UpdateSubmissionState(context.Submission.Id, context.IssuerId, SubmissionState.Deleted, cancellationToken);
        return submission;
    }
}