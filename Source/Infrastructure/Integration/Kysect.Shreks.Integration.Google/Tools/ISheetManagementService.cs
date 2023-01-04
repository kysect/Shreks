namespace Kysect.Shreks.Integration.Google.Tools;

public interface ISheetManagementService
{
    /// <summary>
    ///     Clears all values and formatting of a sheet
    ///     if it exists and creates it otherwise.
    /// </summary>
    /// <returns>Sheet id.</returns>
    Task<int> CreateOrClearSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token);

    /// <returns>Sheet id.</returns>
    Task<int> CreateSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token);

    Task<bool> CheckIfExists(string spreadsheetId, string sheetTitle, CancellationToken token);
}