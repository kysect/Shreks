using Kysect.Shreks.WebApi.Sdk.Exceptions;
using Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;
using Kysect.Shreks.WebUI.AdminPanel.Identity;
using Microsoft.AspNetCore.Components;

namespace Kysect.Shreks.WebUI.AdminPanel.SafeExecution;

public class SafeExecutionBuilder : ISafeExecutionBuilder
{
    private readonly List<Func<Task>> _successHandlers;
    private readonly List<IExceptionHandler> _errorHandlers;
    private readonly IExceptionSink _exceptionSink;
    private readonly IIdentityManager _identityManager;
    private readonly NavigationManager _navigationManager;
    private readonly Func<Task> _action;

    public SafeExecutionBuilder(
        Func<Task> action,
        IExceptionSink exceptionSink,
        IIdentityManager identityManager,
        NavigationManager navigationManager)
    {
        _exceptionSink = exceptionSink;
        _identityManager = identityManager;
        _navigationManager = navigationManager;
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
            IEnumerable<Task> handleTasks = _successHandlers.Select(x => x.Invoke());

            await Task.WhenAll(handleTasks);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            IEnumerable<Task> handleTasks = _errorHandlers.Select(x => x.Handle(e));
            await Task.WhenAll(handleTasks);

            if (e is UnauthorizedException)
            {
                await _identityManager.RemoveIdentityAsync(default);
                _navigationManager.NavigateTo("/");

                return;
            }

            string? message = ShowExceptionDetails ? e.Message : null;

            await _exceptionSink.ConsumeAsync(e, Title, message);
        }
    }
}

public class SafeExecutionBuilder<T> : ISafeExecutionBuilder<T>
{
    private readonly List<Func<T, Task>> _successHandlers;
    private readonly List<IExceptionHandler> _errorHandlers;
    private readonly IExceptionSink _exceptionSink;
    private readonly IIdentityManager _identityManager;
    private readonly NavigationManager _navigationManager;
    private readonly Func<Task<T>> _action;

    public SafeExecutionBuilder(
        Func<Task<T>> action,
        IExceptionSink exceptionSink,
        IIdentityManager identityManager,
        NavigationManager navigationManager)
    {
        _exceptionSink = exceptionSink;
        _identityManager = identityManager;
        _navigationManager = navigationManager;
        _action = action;

        _successHandlers = new List<Func<T, Task>>();
        _errorHandlers = new List<IExceptionHandler>();
    }

    public string? Title { get; set; }
    public bool ShowExceptionDetails { get; set; }

    public void OnFailAsync<TException>(Func<TException, Task> action) where TException : Exception
        => _errorHandlers.Add(new ExceptionHandler<TException>(action));

    public void OnSuccessAsync(Func<T, Task> action)
        => _successHandlers.Add(action);

    public async ValueTask DisposeAsync()
    {
        try
        {
            T value = await _action.Invoke();
            IEnumerable<Task> handleTasks = _successHandlers.Select(x => x.Invoke(value));

            await Task.WhenAll(handleTasks);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            IEnumerable<Task> handleTasks = _errorHandlers.Select(x => x.Handle(e));
            await Task.WhenAll(handleTasks);

            if (e is UnauthorizedException)
            {
                await _identityManager.RemoveIdentityAsync(default);
                _navigationManager.NavigateTo("/");

                return;
            }

            string? message = ShowExceptionDetails ? e.Message : null;

            await _exceptionSink.ConsumeAsync(e, Title, message);
        }
    }
}