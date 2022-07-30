using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Kysect.Shreks.GoogleIntegration.Factories;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Sheets;

namespace Kysect.Shreks.GoogleIntegration.Tools;

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
        
        var createSheetArguments = new CreateSheetArguments
        {
            SheetId = sheetId,
            HeaderSheetRange = new SheetRange(title, sheetId, headerRange),
            DataSheetRange = new SheetRange(title, sheetId, dataRange),
            Editor = new GoogleTableEditor(_service, _spreadsheetId)
        };
        
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

        BatchUpdateSpreadsheetResponse response = await _service.Spreadsheets
            .BatchUpdate(new BatchUpdateSpreadsheetRequest 
            { 
                Requests = new List<Request>
                { 
                    addSheetRequest
                }
            }, _spreadsheetId)
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