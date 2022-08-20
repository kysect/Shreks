using Kysect.Shreks.Integration.Google.Sheets;

namespace Kysect.Shreks.Integration.Google.Tools;

public interface ISheetManagementService
{
    Task CreateOrClearSheetAsync(string spreadsheetId, ISheet sheet, CancellationToken token);

    Task<string> CreateSpreadsheetAsync(string title, CancellationToken token);
}