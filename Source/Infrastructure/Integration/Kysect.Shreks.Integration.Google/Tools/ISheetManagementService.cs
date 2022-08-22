namespace Kysect.Shreks.Integration.Google.Tools;

public interface ISheetManagementService
{
    Task<int> CreateOrClearSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token);

    Task<string> CreateSpreadsheetAsync(string title, CancellationToken token);
}