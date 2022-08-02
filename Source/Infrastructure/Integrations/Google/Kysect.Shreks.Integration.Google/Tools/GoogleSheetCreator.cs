using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Sheets;

namespace Kysect.Shreks.Integration.Google.Tools;

public class GoogleSheetCreator
{
    private readonly SheetsService _service;
    private readonly string _spreadsheetId;

    public GoogleSheetCreator(SheetsService service, string spreadsheetId)
    {
        _service = service;
        _spreadsheetId = spreadsheetId;
    }

    public async Task<TSheet> GetOrCreateSheetAsync<TSheet>(
        ISheetFactory<TSheet> sheetFactory,
        SheetDescriptor descriptor,
        CancellationToken token)
        where TSheet : ISheet
    {
        (string title, string headerRange, string dataRange) = descriptor;

        int sheetId = await GetSheetIdAsync(title, token) ?? await CreateSheetAsync(title, token);

        var createSheetArguments = new CreateSheetArguments(
            sheetId,
            new SheetRange(title, sheetId, headerRange),
            new SheetRange(title, sheetId, dataRange),
            new GoogleTableEditor(_service, _spreadsheetId));

        return sheetFactory.Create(createSheetArguments);
    }

    public async Task<int?> GetSheetIdAsync(string title, CancellationToken token)
    {
        Spreadsheet spreadSheet = await _service.Spreadsheets
            .Get(_spreadsheetId)
            .ExecuteAsync(token);

        Sheet? sheet = spreadSheet.Sheets.FirstOrDefault(s => s.Properties.Title == title);

        return sheet?.Properties.SheetId;
    }

    public async Task<int> CreateSheetAsync(string title, CancellationToken token)
    {
        var addSheetRequest = new Request
        {
            AddSheet = new AddSheetRequest
            {
                Properties = new SheetProperties
                {
                    Title = title
                }
            }
        };

        var createSheetRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = new List<Request>
            {
                addSheetRequest
            }
        };

        BatchUpdateSpreadsheetResponse response = await _service.Spreadsheets
            .BatchUpdate(createSheetRequest, _spreadsheetId)
            .ExecuteAsync(token);

        return response
            .Replies
            .First()
            .AddSheet
            .Properties
            .SheetId!
            .Value;
    }
}