namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Identity;

public interface IIdentityService
{
    Task LoginAsync(string username, string password, CancellationToken cancellationToken);
}