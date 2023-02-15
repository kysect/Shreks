namespace ITMO.Dev.ASAP.DeveloperEnvironment;

public record DeveloperEnvironmentSeedingRequest(
    string Environment,
    string Organization,
    string TemplateRepository,
    string MentorTeamName,
    IReadOnlyList<string> Users);