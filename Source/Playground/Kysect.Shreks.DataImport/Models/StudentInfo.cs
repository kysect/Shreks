namespace Kysect.Shreks.DataImport.Models;

public readonly record struct StudentInfo(
    string FullName,
    string Group,
    string GithubUsername,
    string TelegramTag,
    int IsuNumber,
    DateTime Submitted);