namespace ITMO.Dev.ASAP.Integration.Google.Tools;

public interface ISpreadsheetManagementService
{
    /// <summary>
    ///     Creates spreadsheet in drive,
    ///     configured by DriveParentProvider
    /// </summary>
    /// <returns>Spreadsheet id</returns>
    Task<string> CreateSpreadsheetAsync(string title, CancellationToken token);
}