namespace Kysect.Shreks.WebApi;

public class TestEnvConfiguration
{
    public string Organization { get; init; }
    public string TemplateRepository { get; init; }
    public List<string> Users { get; init; }
    public bool UseDummyGithubImplementation { get; init; }
}