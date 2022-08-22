using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Kysect.Shreks.Integration.Google.Providers;
using File = Google.Apis.Drive.v3.Data.File;

namespace Kysect.Shreks.Integration.Google.Tools;

public class SheetManagementService : ISheetManagementService
{
    private const string AllFields = "*";
    private const string SpreadsheetType = "application/vnd.google-apps.spreadsheet";

    private static readonly Permission AnyoneViewerPermission = new()
    {
        Type = "anyone",
        Role = "reader"
    };

    private readonly SheetsService _sheetsService;
    private readonly DriveService _driveService;
    private readonly ITablesParentsProvider _tablesParentsProvider;

    public SheetManagementService(
        SheetsService sheetsService,
        DriveService driveService,
        ITablesParentsProvider tablesParentsProvider)
    {
        _sheetsService = sheetsService;
        _driveService = driveService;
        _tablesParentsProvider = tablesParentsProvider;
    }

    public async Task<int> CreateOrClearSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token)
    {
        int? sheetId = await GetSheetIdAsync(spreadsheetId, sheetTitle, token);

        if (sheetId is null)
        {
            return await CreateSheetAsync(spreadsheetId, sheetTitle, token);
        }
        
        await ClearSheetAsync(spreadsheetId, sheetId.Value, token);
        return sheetId.Value;
    }

    public async Task<string> CreateSpreadsheetAsync(string title, CancellationToken token)
    {
        var spreadsheetToCreate = new File
        {
            Parents = _tablesParentsProvider.GetParents(),
            MimeType = SpreadsheetType,
            Name = title
        };

        var spreadsheet = await _driveService.Files
            .Create(spreadsheetToCreate)
            .ExecuteAsync(token);

        string spreadsheetId = spreadsheet.Id;

        await _driveService.Permissions
            .Create(AnyoneViewerPermission, spreadsheetId)
            .ExecuteAsync(token);

        return spreadsheetId;
    }

    private async Task<int?> GetSheetIdAsync(string spreadsheetId, string title, CancellationToken token)
    {
        Spreadsheet spreadsheet = await _sheetsService.Spreadsheets
            .Get(spreadsheetId)
            .ExecuteAsync(token);

        Sheet? sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == title);

        return sheet?.Properties.SheetId;
    }

    private async Task<int> CreateSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token)
    {
        var addSheetRequest = new Request
        {
            AddSheet = new AddSheetRequest
            {
                Properties = new SheetProperties
                {
                    Title = sheetTitle
                }
            }
        };

        var batchUpdateResponse = await ExecuteBatchUpdateAsync(spreadsheetId, addSheetRequest, token);
        var addedSheetProperties = batchUpdateResponse.Replies[0].AddSheet.Properties;

        return addedSheetProperties.SheetId!.Value;
    }

    private async Task ClearSheetAsync(string spreadsheetId, int sheetId, CancellationToken token)
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

        await ExecuteBatchUpdateAsync(spreadsheetId, requests, token);
    }

    private async Task<BatchUpdateSpreadsheetResponse> ExecuteBatchUpdateAsync(
        string spreadsheetId,
        Request request,
        CancellationToken token)
    {
        var requests = new[] { request };
        return await ExecuteBatchUpdateAsync(spreadsheetId, requests, token);
    }

    private async Task<BatchUpdateSpreadsheetResponse> ExecuteBatchUpdateAsync(
        string spreadsheetId,
        IList<Request> requests,
        CancellationToken token)
    {
        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = requests
        };

        return await _sheetsService.Spreadsheets
            .BatchUpdate(batchUpdateRequest, spreadsheetId)
            .ExecuteAsync(token);
    }
}