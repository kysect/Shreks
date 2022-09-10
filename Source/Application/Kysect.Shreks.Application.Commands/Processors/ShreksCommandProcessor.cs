using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.UserCommands.Abstractions.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Microsoft.Extensions.Logging;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Application.UserCommands.Abstractions;

namespace Kysect.Shreks.Application.Commands.Processors;

public class ShreksCommandProcessor : IShreksCommandProcessor
{
    private readonly ICommandContextFactory _commandContextFactory;

    public ShreksCommandProcessor(ICommandContextFactory commandContextFactory)
    {
        _commandContextFactory = commandContextFactory;
    }

    public async Task<SubmissionRateDto> Rate(RateCommand rateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        string message = $"Handle /rate command from {context.IssuerId} with arguments:" +
                         $" {{ RatingPercent: {rateCommand.RatingPercent}," +
                         $" ExtraPoints: {rateCommand.ExtraPoints}}}";
        context.Log.LogInformation(message);

        var submissionId = context.Submission.Id;
        var command = new UpdateSubmissionPoints.Command(submissionId, context.IssuerId, rateCommand.RatingPercent, rateCommand.ExtraPoints);
        var response = await context.Mediator.Send(command, cancellationToken);
        return response.SubmissionRate;
    }

    public async Task<SubmissionRateDto> Update(UpdateCommand updateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        var SubmissionCode = updateCommand.SubmissionCode;
        var RatingPercent = updateCommand.RatingPercent;
        var ExtraPoints = updateCommand.ExtraPoints;
        var DateStr = updateCommand.DateStr;

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

    public async Task<string> Help(HelpCommand helpCommand, CancellationToken token)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(token);

        context.Log.LogDebug($"Handle /help command from {context.IssuerId}");
        string helpString = @"
Команды:

- `/help`
- `/deactivate` - перевод сабмишена в неактивное состояние, чтобы обозначить, что он не готов к сдаче
- `/activate` - перевод сабмишена обратно в активное состояние
- `/create-submission` - команда на случай, если был создан PR, но сабмишен не был автоматически создан
- `/delete` - удаление сабмишена. Стоит использовать только в ситуациях, когда сабмишен был создан случайно.

Команды только для преподавателя:

- `/rate [RatingPercent] {ExtraPoints}`. Например, `/rate 90 2` - это 90% баллов за работу +2 дополнительных баллов;
- `/update [Submission number] {-r RatingPercent} {-e ExtraPoints} {-d Date}`. Например:
  - `/update 1 -d 27.09.2022` обновит сабмишен с номером 1 и поставит ему дату 27.09
  - `/update 2 -r 100 -e 5` обновит сабмишен с номером 2 и поставит ему 100% рейта и 5 дополнительных баллов

Обрабатываемые ивенты:

- Создание или переоткрытие PR - создаёт сабмишен
- Добавление нового комита в PR - обновляет дату сабмишена на текущую";

        return helpString;
    }

    public async Task<SubmissionDto> Activate(ActivateCommand activateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        context.Log.LogInformation($"Handle /activate command for submission {context.Submission.Id} from user {context.IssuerId}");
        var command = new UpdateSubmissionState.Command(
            context.IssuerId, context.Submission.Id, SubmissionStateDto.Active);

        var response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public async Task<SubmissionDto> Deactivate(DeactivateCommand deactivateCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        context.Log.LogInformation($"Handle /deactivate command for submission {context.Submission.Id} from user {context.IssuerId}");
        var command = new UpdateSubmissionState.Command(
            context.IssuerId, context.Submission.Id, SubmissionStateDto.Inactive);

        var response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
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

    public async Task<SubmissionDto> Delete(DeleteCommand deleteCommand, CancellationToken cancellationToken)
    {
        var context = await _commandContextFactory.CreateSubmissionContext(cancellationToken);

        var command = new UpdateSubmissionState.Command(
            context.IssuerId, context.Submission.Id, SubmissionStateDto.Deleted);

        var response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
    }
}