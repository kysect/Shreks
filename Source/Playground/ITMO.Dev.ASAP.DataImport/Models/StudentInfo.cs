namespace ITMO.Dev.ASAP.DataImport.Models;

public readonly record struct StudentInfo(
    string FullName,
    string Group,
    string GithubUsername,
    string TelegramTag,
    int IsuNumber,
    DateTime Submitted);