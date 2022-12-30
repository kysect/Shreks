using Kysect.Shreks.WebUI.AdminPanel.Identity;
using Kysect.Shreks.WebUI.AdminPanel.SafeExecution;
using Microsoft.AspNetCore.Components;

namespace Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;

public class SafeExecutor : ISafeExecutor
{
    private readonly IExceptionSink _exceptionSink;
    private readonly IIdentityManager _identityManager;
    private readonly NavigationManager _navigationManager;

    public SafeExecutor(
        IExceptionSink exceptionSink,
        IIdentityManager identityManager,
        NavigationManager navigationManager)
    {
        _exceptionSink = exceptionSink;
        _identityManager = identityManager;
        _navigationManager = navigationManager;
    }

    public ISafeExecutionBuilder Execute(Func<Task> action)
        => new SafeExecutionBuilder(action, _exceptionSink, _identityManager, _navigationManager);

    public ISafeExecutionBuilder<T> Execute<T>(Func<Task<T>> action)
        => new SafeExecutionBuilder<T>(action, _exceptionSink, _identityManager, _navigationManager);
}