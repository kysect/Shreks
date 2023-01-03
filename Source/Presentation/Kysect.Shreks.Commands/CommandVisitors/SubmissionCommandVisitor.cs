using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Commands.Contexts;
using Kysect.Shreks.Commands.Result;
using Kysect.Shreks.Commands.SubmissionCommands;
using Kysect.Shreks.Common.Resources;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Commands.CommandVisitors;

public class SubmissionCommandVisitor : ISubmissionCommandVisitor
{
    private readonly CancellationToken _cancellationToken;
    private readonly ISubmissionCommandContextFactory _contextFactory;
    private readonly ILogger _logger;

    public SubmissionCommandVisitor(
        ISubmissionCommandContextFactory contextFactory,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        _contextFactory = contextFactory;
        _logger = logger;
        _cancellationToken = cancellationToken;
    }

    public IShreksCommandResult? Result { get; private set; }

    public async Task VisitAsync(ActivateCommand command)
    {
        SubmissionContext context = await _contextFactory.SubmissionContextAsync(_cancellationToken);
        await command.ExecuteAsync(context, _logger, _cancellationToken);

        Result = BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionActivatedSuccessfully());
    }

    public async Task VisitAsync(BanCommand command)
    {
        UpdateContext context = await _contextFactory.UpdateContextAsync(_cancellationToken);
        SubmissionDto submission = await command.ExecuteAsync(context, _logger, _cancellationToken);

        Result = BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionBanned(submission.Id));
    }

    public async Task VisitAsync(CreateSubmissionCommand command)
    {
        CreateSubmissionContext context = await _contextFactory.CreateSubmissionContextAsync(_cancellationToken);
        SubmissionRateDto submission = await command.ExecuteAsync(context, _logger, _cancellationToken);

        string message = UserCommandProcessingMessage.SubmissionCreated(submission.ToDisplayString());
        Result = BaseShreksCommandResult.Success(message);
    }

    public async Task VisitAsync(DeactivateCommand command)
    {
        SubmissionContext context = await _contextFactory.SubmissionContextAsync(_cancellationToken);
        await command.ExecuteAsync(context, _logger, _cancellationToken);

        Result = BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionDeactivatedSuccessfully());
    }

    public async Task VisitAsync(DeleteCommand command)
    {
        SubmissionContext context = await _contextFactory.SubmissionContextAsync(_cancellationToken);
        await command.ExecuteAsync(context, _logger, _cancellationToken);

        Result = BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionDeletedSuccessfully());
    }

    public async Task VisitAsync(HelpCommand command)
    {
        BaseContext context = await _contextFactory.BaseContextAsync(_cancellationToken);
        string result = await command.ExecuteAsync(context, _logger, _cancellationToken);

        Result = BaseShreksCommandResult.Success(result);
    }

    public async Task VisitAsync(MarkReviewedCommand command)
    {
        SubmissionContext context = await _contextFactory.SubmissionContextAsync(_cancellationToken);
        await command.ExecuteAsync(context, _logger, _cancellationToken);

        Result = BaseShreksCommandResult.Success(UserCommandProcessingMessage.SubmissionMarkedAsReviewed());
    }

    public async Task VisitAsync(RateCommand command)
    {
        SubmissionContext context = await _contextFactory.SubmissionContextAsync(_cancellationToken);
        SubmissionRateDto submission = await command.ExecuteAsync(context, _logger, _cancellationToken);

        string message = UserCommandProcessingMessage.SubmissionRated(submission.ToDisplayString());

        Result = BaseShreksCommandResult.Success(message);
    }

    public async Task VisitAsync(UpdateCommand command)
    {
        UpdateContext context = await _contextFactory.UpdateContextAsync(_cancellationToken);
        SubmissionRateDto submission = await command.ExecuteAsync(context, _logger, _cancellationToken);

        string message = UserCommandProcessingMessage.SubmissionUpdated(submission.ToDisplayString());

        Result = BaseShreksCommandResult.Success(message);
    }
}