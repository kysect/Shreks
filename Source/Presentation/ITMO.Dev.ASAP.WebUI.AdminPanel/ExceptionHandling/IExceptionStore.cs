using ITMO.Dev.ASAP.WebUI.AdminPanel.Models;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.ExceptionHandling;

public interface IExceptionStore
{
    IReadOnlyCollection<ExceptionMessage> Exceptions { get; }

    void Dismiss(ExceptionMessage exception);

    event Action<ExceptionMessage> ExceptionAdded;

    event Action<ExceptionMessage> ExceptionDismissed;
}