namespace Kysect.Shreks.Application.Abstractions.Github;

public interface IOrganizationDetailsProvider
{
    Task<IReadOnlyCollection<string>> GetOrganizationOwners(string organizationName);
}