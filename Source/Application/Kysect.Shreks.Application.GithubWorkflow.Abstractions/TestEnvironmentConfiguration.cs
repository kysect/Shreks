namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public class TestEnvironmentConfiguration
{
    public string Organization { get; init; }
    public string TemplateRepository { get; init; }
    public List<string> Users { get; init; }
}