using Kysect.Shreks.WebUI.AdminPanel.Models;
using Kysect.Shreks.WebUI.AdminPanel.Tools;

namespace Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;

public class ExceptionManager : IExceptionSink, IExceptionStore
{
    private readonly HashSet<ExceptionMessage> _exceptions;
    private readonly ExceptionDisplayConfiguration _configuration;

    public ExceptionManager(ExceptionDisplayConfiguration configuration)
    {
        _configuration = configuration;
        var comparer = EqualityComparerFactory.Create<ExceptionMessage>((a, b) => a.Exception.Equals(b.Exception));
        _exceptions = new HashSet<ExceptionMessage>(comparer);
    }

    public IReadOnlyCollection<ExceptionMessage> Exceptions => _exceptions;

    public async ValueTask ConsumeAsync(Exception exception, string? title, string? message)
    {
        var value = new ExceptionMessage(title, message, exception);
        _exceptions.Add(value);
        OnExceptionAdded(value);

        await Task.Delay(_configuration.PopupLifetime);
        Dismiss(new ExceptionMessage(title, message, exception));
    }

    public void Dismiss(ExceptionMessage exception)
    {
        _exceptions.Remove(exception);
        OnExceptionDismissed(exception);
    }

    public event Action<ExceptionMessage>? ExceptionAdded;
    public event Action<ExceptionMessage>? ExceptionDismissed;

    protected virtual void OnExceptionAdded(ExceptionMessage obj)
        => ExceptionAdded?.Invoke(obj);

    protected virtual void OnExceptionDismissed(ExceptionMessage obj)
        => ExceptionDismissed?.Invoke(obj);
}