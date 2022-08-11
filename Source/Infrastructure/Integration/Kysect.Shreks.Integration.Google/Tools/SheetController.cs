using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Sheets;

namespace Kysect.Shreks.Integration.Google.Tools;

public class SheetController : ISheetController
{
    private const string AllFields = "*";

    private readonly SheetsService _sheetsService;
    private readonly ISpreadsheetIdProvider _spreadsheetIdProvider;

    public SheetController(SheetsService sheetsService, ISpreadsheetIdProvider spreadsheetIdProvider)
    {
        _sheetsService = sheetsService;
        _spreadsheetIdProvider = spreadsheetIdProvider;
    }

    public async Task CreateOrClearSheetAsync(ISheet sheet, CancellationToken token)
    {
        bool sheetExists = await CheckSheetExistsAsync(sheet.Title, token);

        if (sheetExists)
        {
            await ClearSheetAsync(sheet.Id, token);
        }
        else
        {
            await CreateSheetAsync(sheet, token);
        }
    }

    private async Task<bool> CheckSheetExistsAsync(string title, CancellationToken token)
    {
        Spreadsheet spreadSheet = await _sheetsService.Spreadsheets
            .Get(_spreadsheetIdProvider.SpreadsheetId)
            .ExecuteAsync(token);

        Sheet? sheet = spreadSheet.Sheets.FirstOrDefault(s => s.Properties.Title == title);

        return sheet is not null;
    }

    private async Task CreateSheetAsync(ISheet sheet, CancellationToken token)
    {
        var addSheetRequest = new Request
        {
            AddSheet = new AddSheetRequest
            {
                Properties = new SheetProperties
                {
                    Title = sheet.Title,
                    SheetId = sheet.Id
                }
            }
        };

        await ExecuteBatchUpdateAsync(addSheetRequest, token);
    }

    private async Task ClearSheetAsync(int sheetId, CancellationToken token)
    {
        var allFieldsGridRange = new GridRange
        {
            StartRowIndex = 0,
            StartColumnIndex = 0,
            SheetId = sheetId
        };

        var updateCellsRequest = new Request
        {
            UpdateCells = new UpdateCellsRequest
            {
                Range = allFieldsGridRange,
                Fields = AllFields
            }
        };

        var unmergeCellsRequest = new Request
        {
            UnmergeCells = new UnmergeCellsRequest
            {
                Range = allFieldsGridRange
            }
        };

        var requests = new List<Request>
        {
            updateCellsRequest,
            unmergeCellsRequest
        };

        await ExecuteBatchUpdateAsync(requests, token);
    }

    private Task ExecuteBatchUpdateAsync(Request request, CancellationToken token)
        => ExecuteBatchUpdateAsync(new[] { request }, token);

    private async Task ExecuteBatchUpdateAsync(IList<Request> requests, CancellationToken token)
    {
        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = requests };

        await _sheetsService.Spreadsheets
            .BatchUpdate(batchUpdateRequest, _spreadsheetIdProvider.SpreadsheetId)
            .ExecuteAsync(token);
    }
}