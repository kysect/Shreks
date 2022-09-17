namespace Kysect.Shreks.Integration.Google.Providers;

public class DriveParentProvider : ITablesParentsProvider
{
    private readonly IList<string> _parents;

    public DriveParentProvider(string driveId)
    {
        _parents = new List<string> { driveId };
    }

    public IList<string> GetParents()
    {
        return _parents;
    }
}