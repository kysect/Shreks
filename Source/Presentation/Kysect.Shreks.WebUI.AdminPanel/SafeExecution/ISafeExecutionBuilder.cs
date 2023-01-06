namespace Kysect.Shreks.WebUI.AdminPanel.SafeExecution;

public interface ISafeExecutionBuilder : IAsyncDisposable
{
    string? Title { get; set; }

    bool ShowExceptionDetails { get; set; }

    void OnFailAsync<TException>(Func<TException, Task> action) where TException : Exception;

    void OnSuccessAsync(Func<Task> action);
}

public interface ISafeExecutionBuilder<out T> : IAsyncDisposable
{
    string? Title { get; set; }

    bool ShowExceptionDetails { get; set; }

    void OnFailAsync<TException>(Func<TException, Task> action) where TException : Exception;

    void OnSuccessAsync(Func<T, Task> action);
}