namespace ITMO.Dev.ASAP.WebUI.AdminPanel.SafeExecution;

public interface IExceptionHandler
{
    Task Handle(Exception exception);
}