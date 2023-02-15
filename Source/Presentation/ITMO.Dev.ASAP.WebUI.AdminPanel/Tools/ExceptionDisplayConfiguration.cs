namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Tools;

public record ExceptionDisplayConfiguration(TimeSpan PopupLifetime, bool ShowExceptionDetails = true);