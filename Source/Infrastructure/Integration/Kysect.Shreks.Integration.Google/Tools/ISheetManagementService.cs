namespace Kysect.Shreks.Integration.Google.Tools;

public interface ISheetManagementService
{
    /// <summary>
    ///     Clears all values and formatting of a sheet
    ///     if it exists and creates it otherwise
    /// </summary>
    /// <returns>Sheet id</returns>
    Task<int> CreateOrClearSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token);

    /// <summary>
    ///     Creates spreadsheet in drive,
    ///     configured by DriveParentProvider
    /// </summary>
    /// <returns>Spreadsheet id</returns>
    Task<string> CreateSpreadsheetAsync(string title, CancellationToken token);

    Task<bool> CheckIfExists(string spreadsheetId, string sheetTitle, CancellationToken token);
}