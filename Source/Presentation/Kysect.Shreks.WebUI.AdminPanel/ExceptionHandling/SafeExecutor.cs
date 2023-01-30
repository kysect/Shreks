using Kysect.Shreks.WebUI.AdminPanel.Identity;
using Kysect.Shreks.WebUI.AdminPanel.SafeExecution;
using Kysect.Shreks.WebUI.AdminPanel.Tools;
using Microsoft.AspNetCore.Components;

namespace Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;

public class SafeExecutor : ISafeExecutor
{
    private readonly IExceptionSink _exceptionSink;
    private readonly IIdentityManager _identityManager;
    private readonly NavigationManager _navigationManager;
    private readonly EnvironmentConfiguration _configuration;

    public SafeExecutor(
        IExceptionSink exceptionSink,
        IIdentityManager identityManager,
        NavigationManager navigationManager,
        EnvironmentConfiguration configuration)
    {
        _exceptionSink = exceptionSink;
        _identityManager = identityManager;
        _navigationManager = navigationManager;
        _configuration = configuration;
    }

    public ISafeExecutionBuilder Execute(Func<Task> action)
        => new SafeExecutionBuilder(action, _exceptionSink, _identityManager, _navigationManager, _configuration);

    public ISafeExecutionBuilder<T> Execute<T>(Func<Task<T>> action)
        => new SafeExecutionBuilder<T>(action, _exceptionSink, _identityManager, _navigationManager, _configuration);
}