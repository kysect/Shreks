using Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;

namespace Kysect.Shreks.WebUI.AdminPanel.SafeExecution;

public class SafeExecutionBuilder : ISafeExecutionBuilder
{
    private readonly List<Func<Task>> _successHandlers;
    private readonly List<IExceptionHandler> _errorHandlers;
    private readonly IExceptionSink _exceptionSink;
    private readonly Func<Task> _action;

    public SafeExecutionBuilder(Func<Task> action, IExceptionSink exceptionSink)
    {
        _exceptionSink = exceptionSink;
        _action = action;

        _successHandlers = new List<Func<Task>>();
        _errorHandlers = new List<IExceptionHandler>();
    }

    public string? Title { get; set; }
    public bool ShowExceptionDetails { get; set; }

    public void OnFailAsync<TException>(Func<TException, Task> action) where TException : Exception
        => _errorHandlers.Add(new ExceptionHandler<TException>(action));

    public void OnSuccessAsync(Func<Task> action)
        => _successHandlers.Add(action);

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _action.Invoke();

            var handleTasks = _successHandlers.Select(x => x.Invoke());
            await Task.WhenAll(handleTasks);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            var handleTasks = _errorHandlers.Select(x => x.Handle(e));
            await Task.WhenAll(handleTasks);

            var message = ShowExceptionDetails ? e.Message : null;

            _exceptionSink.Consume(e, Title, message);
        }
    }
}

public class SafeExecutionBuilder<T> : ISafeExecutionBuilder<T>
{
    private readonly List<Func<T, Task>> _successHandlers;
    private readonly List<IExceptionHandler> _errorHandlers;
    private readonly IExceptionSink _exceptionSink;
    private readonly Func<Task<T>> _action;

    public SafeExecutionBuilder(Func<Task<T>> action, IExceptionSink exceptionSink)
    {
        _exceptionSink = exceptionSink;
        _action = action;

        _successHandlers = new List<Func<T, Task>>();
        _errorHandlers = new List<IExceptionHandler>();
    }

    public string? Title { get; set; }
    public bool ShowExceptionDetails { get; set; }

    public void OnFailAsync<TException>(Func<TException, Task> action) where TException : Exception
        => _errorHandlers.Add(new ExceptionHandler<TException>(action));

    public void SuccessAsync(Func<T, Task> action)
        => _successHandlers.Add(action);

    public async ValueTask DisposeAsync()
    {
        try
        {
            var value = await _action.Invoke();

            var handleTasks = _successHandlers.Select(x => x.Invoke(value));
            await Task.WhenAll(handleTasks);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            var handleTasks = _errorHandlers.Select(x => x.Handle(e));
            await Task.WhenAll(handleTasks);

            var message = ShowExceptionDetails ? e.Message : null;

            _exceptionSink.Consume(e, Title, message);
        }
    }
}