namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Models;

public readonly record struct ExceptionMessage(string? Title, string? Message, Exception Exception);