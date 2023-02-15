using ITMO.Dev.ASAP.WebUI.AdminPanel.SafeExecution;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.ExceptionHandling;

public interface ISafeExecutor
{
    ISafeExecutionBuilder Execute(Func<Task> action);

    ISafeExecutionBuilder<T> Execute<T>(Func<Task<T>> action);
}