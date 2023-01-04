namespace Kysect.Shreks.WebUI.AdminPanel.Exceptions;

public abstract class AdminPanelException : Exception
{
    protected AdminPanelException(string? message)
        : base(message) { }
}