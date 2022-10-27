namespace Kysect.Shreks.WebUI.AdminPanel.Exceptions;

public class UnauthorizedException : AdminPanelException
{
    public UnauthorizedException() : base("Current user is unauthorized") { }
}