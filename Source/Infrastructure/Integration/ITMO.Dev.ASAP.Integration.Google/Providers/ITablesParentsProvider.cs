namespace ITMO.Dev.ASAP.Integration.Google.Providers;

public interface ITablesParentsProvider
{
    IList<string> GetParents();
}