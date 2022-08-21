using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;

namespace Kysect.Shreks.Integration.Google.Tools;

public static class GoogleRequiredScopes
{
    public static IReadOnlyCollection<string> GetRequiredScopes()
    {
        return new[]
        {
            SheetsService.Scope.Spreadsheets,
            DriveService.Scope.Drive
        };
    }
}