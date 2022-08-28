using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/help")]
public class HelpCommand : IShreksCommand<BaseContext, string>
{
    public Task<string> ExecuteAsync(BaseContext context, CancellationToken cancellationToken)
    {
        context.Log.LogDebug($"Handle /help command from {context.IssuerId}");
        string helpString = @"
Команды:

- `/help`
- `/to-draft` - перевод сабмишена в неактивное состояние, чтобы обозначить, что он не готов к сдаче
- `/to-active` - перевод сабмишена обратно в активное состояние
- `/create-submission` - команда на случай, если был создан PR, но сабмишен не был автоматически создан

Команды только для преподавателя:

- `/rate [RatingPercent] {ExtraPoints}`
- `/update-rate [Submission number] {RatingPercent} {ExtraPoints}`
- `/update-date [Submission number] {RatingPercent} {ExtraPoints}`

Обрабатываемые ивенты:

- Создание или переоткрытие PR - создаёт сабмишен
- Добавление нового комита в PR - обновляет дату сабмишена на текущую";

        return Task.FromResult(helpString);
    }

    public Task<TResult> AcceptAsync<TResult>(IShreksCommandVisitor<TResult> visitor) where TResult : IShreksCommandResult
    {
        return visitor.VisitAsync(this);
    }
}