using Kysect.Shreks.WebUI.AdminPanel.SafeExecution;

namespace Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;

public interface ISafeExecutor
{
    ISafeExecutionBuilder Execute(Func<Task> action);
    ISafeExecutionBuilder<T> Execute<T>(Func<Task<T>> action);
}