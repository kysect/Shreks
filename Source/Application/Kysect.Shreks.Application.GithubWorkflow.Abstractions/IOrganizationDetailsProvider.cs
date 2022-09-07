namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public interface IOrganizationDetailsProvider
{
    Task<IReadOnlyCollection<string>> GetOrganizationOwners(string organizationName);
}