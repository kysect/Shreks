namespace Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;

public interface IExceptionSink
{
    void Consume(Exception exception, string? title, string? message);
}