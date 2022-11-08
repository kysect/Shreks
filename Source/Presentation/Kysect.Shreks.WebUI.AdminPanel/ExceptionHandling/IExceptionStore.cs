using Kysect.Shreks.WebUI.AdminPanel.Models;

namespace Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;

public interface IExceptionStore
{
    IReadOnlyCollection<ExceptionMessage> Exceptions { get; }
    void Dismiss(ExceptionMessage exception);

    event Action<ExceptionMessage> ExceptionAdded;
    event Action<ExceptionMessage> ExceptionDismissed;
}