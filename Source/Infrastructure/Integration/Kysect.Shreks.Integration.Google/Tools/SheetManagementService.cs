using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Sheets;
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
    private readonly IDriveIdProvider _driveIdProvider;

    public SheetManagementService(
        SheetsService sheetsService,
        DriveService driveService,
        IDriveIdProvider driveIdProvider)
    {
        _sheetsService = sheetsService;
        _driveService = driveService;
        _driveIdProvider = driveIdProvider;
    }

    public async Task CreateOrClearSheetAsync(string spreadsheetId, ISheet sheet, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(sheet);

        bool sheetExists = await CheckSheetExistsAsync(spreadsheetId, sheet.Title, token);

        if (sheetExists)
        {
            await ClearSheetAsync(spreadsheetId, sheet.Id, token);
        }
        else
        {
            await CreateSheetAsync(spreadsheetId, sheet, token);
        }
    }

    public async Task<string> CreateSpreadsheetAsync(string title, CancellationToken token)
    {
        var spreadsheetToCreate = new File
        {
            Parents = new List<string> { _driveIdProvider.GetDriveId() },
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

    private async Task<bool> CheckSheetExistsAsync(string spreadsheetId, string title, CancellationToken token)
    {
        Spreadsheet spreadsheet = await _sheetsService.Spreadsheets
            .Get(spreadsheetId)
            .ExecuteAsync(token);

        Sheet? sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == title);

        return sheet is not null;
    }

    private async Task CreateSheetAsync(string spreadsheetId, ISheet sheet, CancellationToken token)
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

        await ExecuteBatchUpdateAsync(spreadsheetId, addSheetRequest, token);
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

    private Task ExecuteBatchUpdateAsync(string spreadsheetId, Request request, CancellationToken token)
        => ExecuteBatchUpdateAsync(spreadsheetId, new[] { request }, token);

    private async Task ExecuteBatchUpdateAsync(string spreadsheetId, IList<Request> requests, CancellationToken token)
    {
        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = requests };

        await _sheetsService.Spreadsheets
            .BatchUpdate(batchUpdateRequest, spreadsheetId)
            .ExecuteAsync(token);
    }
}