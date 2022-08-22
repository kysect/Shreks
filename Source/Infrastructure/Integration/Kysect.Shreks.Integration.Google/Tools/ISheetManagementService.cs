namespace Kysect.Shreks.Integration.Google.Tools;

public interface ISheetManagementService
{
    /// <summary>
    /// Clears all values and formatting of a sheet
    /// if it exists and creates it otherwise
    /// </summary>
    /// <returns>Sheet id</returns>
    Task<int> CreateOrClearSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token);

    Task<string> CreateSpreadsheetAsync(string title, CancellationToken token);
}