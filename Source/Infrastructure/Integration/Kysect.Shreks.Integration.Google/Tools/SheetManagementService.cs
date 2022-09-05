using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Sheets;
using Microsoft.Extensions.Logging;
using File = Google.Apis.Drive.v3.Data.File;

namespace Kysect.Shreks.Integration.Google.Tools;

/// <inheritdoc/>
public class SheetManagementService : ISheetManagementService
{
    private const string AllFields = "*";
    private const string SpreadsheetType = "application/vnd.google-apps.spreadsheet";

    private const int DefaultSheetId = LabsSheet.Id;
    private const string DefaultSheetTitle = LabsSheet.Title;
    
    private const string UpdateTitle = "title";

    private static readonly Permission AnyoneViewerPermission = new()
    {
        Type = "anyone",
        Role = "reader"
    };

    private readonly SheetsService _sheetsService;
    private readonly DriveService _driveService;
    private readonly ITablesParentsProvider _tablesParentsProvider;
    private readonly ISheetTitleComparer _sheetTitleComparer;
    private readonly ILogger<SheetManagementService> _logger;

    public SheetManagementService(
        SheetsService sheetsService,
        DriveService driveService,
        ITablesParentsProvider tablesParentsProvider,
        ISheetTitleComparer sheetTitleComparer,
        ILogger<SheetManagementService> logger)
    {
        _sheetsService = sheetsService;
        _driveService = driveService;
        _tablesParentsProvider = tablesParentsProvider;
        _sheetTitleComparer = sheetTitleComparer;
        _logger = logger;
    }

    public async Task<int> CreateOrClearSheetAsync(string spreadsheetId, string sheetTitle, CancellationToken token)
    {
        int? sheetId = await GetSheetIdAsync(spreadsheetId, sheetTitle, token);

        if (sheetId is null)
        {
            sheetId = await CreateSheetAsync(spreadsheetId, sheetTitle, token);
            await SortSheetsAsync(spreadsheetId, token);
        }
        else
        {
            await ClearSheetAsync(spreadsheetId, sheetId.Value, token);
        }
        
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

        _logger.LogDebug($"Create file {title} on Google drive.");

        File spreadsheetFile = await _driveService.Files
            .Create(spreadsheetToCreate)
            .ExecuteAsync(token);

        string spreadsheetId = spreadsheetFile.Id;
        
        await ConfigureDefaultSheetAsync(spreadsheetId, token);

        _logger.LogDebug($"Update file permission {title}.");

        await _driveService.Permissions
            .Create(AnyoneViewerPermission, spreadsheetId)
            .ExecuteAsync(token);

        return spreadsheetId;
    }

    public async Task<bool> CheckIfExists(string spreadsheetId, string sheetTitle, CancellationToken token)
    {
        int? sheetId = await GetSheetIdAsync(spreadsheetId, sheetTitle, token);
        return sheetId is not null;
    }

    private async Task ConfigureDefaultSheetAsync(string spreadsheetId, CancellationToken token)
    {
        var defaultSheetProperties = new SheetProperties
        {
            SheetId = DefaultSheetId,
            Title = DefaultSheetTitle
        };

        var updatePropertiesRequest = new Request
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = defaultSheetProperties,
                Fields = UpdateTitle
            }
        };

        _logger.LogDebug($"Configure default sheet for {spreadsheetId}.");
        
        await ExecuteBatchUpdateAsync(spreadsheetId, updatePropertiesRequest, token);
    }

    private async Task<int?> GetSheetIdAsync(string spreadsheetId, string title, CancellationToken token)
    {
        IList<Sheet> sheets = await GetSheetsAsync(spreadsheetId, token);

        Sheet? sheet = sheets.FirstOrDefault(s => s.Properties.Title == title);

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

        _logger.LogDebug($"Create sheet with title {sheetTitle}.");

        var batchUpdateResponse = await ExecuteBatchUpdateAsync(spreadsheetId, addSheetRequest, token);
        var addedSheetProperties = batchUpdateResponse.Replies[0].AddSheet.Properties;

        return addedSheetProperties.SheetId!.Value;
    }

    private async Task SortSheetsAsync(string spreadsheetId, CancellationToken token)
    {
        IList<Sheet> sheets = await GetSheetsAsync(spreadsheetId, token);

        Request[] updateSheetIndexRequests = sheets
            .OrderBy(s => s.Properties.Title, _sheetTitleComparer)
            .Select((s, i) => (Sheet: s, NewIndex: i + 1))
            .Select(t =>
            {
                var newProperties = t.Sheet.Properties;
                newProperties.Index = t.NewIndex;
                
                return new Request
                {
                    UpdateSheetProperties = new UpdateSheetPropertiesRequest
                    {
                        Properties = newProperties,
                        Fields = AllFields
                    }
                };
            })
            .ToArray();

        if (updateSheetIndexRequests.Length is not 0)
        {
            _logger.LogDebug($"Reorder sheets in spreadsheet {spreadsheetId}.");

            await ExecuteBatchUpdateAsync(spreadsheetId, updateSheetIndexRequests, token);
        }
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

        _logger.LogDebug($"Clear sheet with id {sheetId}.");
        await ExecuteBatchUpdateAsync(spreadsheetId, requests, token);
    }

    private async Task<IList<Sheet>> GetSheetsAsync(string spreadsheetId, CancellationToken token)
    {
        _logger.LogDebug($"Request spread sheet with id {spreadsheetId}.");

        Spreadsheet spreadsheet = await _sheetsService.Spreadsheets
            .Get(spreadsheetId)
            .ExecuteAsync(token);

        return spreadsheet.Sheets;
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
        _logger.LogDebug($"Execute batch request for spread sheet with id {spreadsheetId}.");

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = requests
        };

        return await _sheetsService.Spreadsheets
            .BatchUpdate(batchUpdateRequest, spreadsheetId)
            .ExecuteAsync(token);
    }
}