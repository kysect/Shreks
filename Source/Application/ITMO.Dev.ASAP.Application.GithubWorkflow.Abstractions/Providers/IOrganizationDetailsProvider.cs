namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Providers;

public interface IOrganizationDetailsProvider
{
    Task<IReadOnlyCollection<string>> GetOrganizationOwners(string organizationName);
}