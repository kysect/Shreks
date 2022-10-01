namespace Kysect.Shreks.DeveloperEnvironment;

public record DeveloperEnvironmentSeedingRequest(
    string Environment,
    string Organization,
    string TemplateRepository,
    IReadOnlyList<string> Users);