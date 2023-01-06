namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;

public class TestEnvironmentConfiguration
{
    public string Organization { get; init; } = string.Empty;

    public string TemplateRepository { get; init; } = string.Empty;

    public IReadOnlyList<string> Users { get; init; } = new List<string>();
}