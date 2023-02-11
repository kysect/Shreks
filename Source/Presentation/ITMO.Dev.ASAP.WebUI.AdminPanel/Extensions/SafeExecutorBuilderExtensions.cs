using ITMO.Dev.ASAP.WebUI.AdminPanel.SafeExecution;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Extensions;

public static class SafeExecutorBuilderExtensions
{
    public static void OnFailAsync(this ISafeExecutionBuilder builder, Func<Exception, Task> action)
        => builder.OnFailAsync<Exception>(action.Invoke);

    public static void OnFailAsync(this ISafeExecutionBuilder builder, Func<Task> action)
        => builder.OnFailAsync(_ => action.Invoke());
}