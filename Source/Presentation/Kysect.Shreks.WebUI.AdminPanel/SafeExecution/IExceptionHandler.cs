namespace Kysect.Shreks.WebUI.AdminPanel.SafeExecution;

public interface IExceptionHandler
{
    Task Handle(Exception exception);
}