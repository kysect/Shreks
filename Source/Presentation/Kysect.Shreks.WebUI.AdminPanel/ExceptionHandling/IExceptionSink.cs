namespace Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;

public interface IExceptionSink
{
    ValueTask ConsumeAsync(Exception exception, string? title, string? message);
}