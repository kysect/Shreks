namespace Kysect.Shreks.Integration.Google.Providers;

public class ConstDriveIdProvider : IDriveIdProvider
{
    private readonly string _driveId;

    public ConstDriveIdProvider(string driveId)
    {
        _driveId = driveId;
    }

    public string GetDriveId()
        => _driveId;
}