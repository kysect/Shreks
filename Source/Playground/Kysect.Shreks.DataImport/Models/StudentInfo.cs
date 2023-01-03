namespace Kysect.Shreks.DataImport;

public readonly record struct StudentInfo(
    string FullName,
    string Group,
    string GithubUsername,
    string TelegramTag,
    int IsuNumber,
    DateTime Submitted);