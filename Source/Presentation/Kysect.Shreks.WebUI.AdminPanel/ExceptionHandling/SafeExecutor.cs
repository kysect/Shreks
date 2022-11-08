using Kysect.Shreks.WebUI.AdminPanel.SafeExecution;

namespace Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;

public class SafeExecutor : ISafeExecutor
{
    private readonly IExceptionSink _exceptionSink;

    public SafeExecutor(IExceptionSink exceptionSink)
    {
        _exceptionSink = exceptionSink;
    }

    public ISafeExecutionBuilder Execute(Func<Task> action)
        => new SafeExecutionBuilder(action, _exceptionSink);

    public ISafeExecutionBuilder<T> Execute<T>(Func<Task<T>> action)
        => new SafeExecutionBuilder<T>(action, _exceptionSink);
}