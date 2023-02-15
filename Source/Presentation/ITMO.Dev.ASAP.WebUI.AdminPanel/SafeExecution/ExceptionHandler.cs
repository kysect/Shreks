namespace ITMO.Dev.ASAP.WebUI.AdminPanel.SafeExecution;

public class ExceptionHandler<T> : IExceptionHandler where T : Exception
{
    private readonly Func<T, Task> _handler;

    public ExceptionHandler(Func<T, Task> handler)
    {
        _handler = handler;
    }

    public Task Handle(Exception exception)
    {
        if (exception is T typedException)
            return _handler.Invoke(typedException);

        return Task.CompletedTask;
    }
}