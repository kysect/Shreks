using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Sheets;
using Microsoft.Extensions.Logging;
using File = Google.Apis.Drive.v3.Data.File;

namespace Kysect.Shreks.Integration.Google.Tools;

/// <inheritdoc />
public class SpreadsheetManagementService : ISpreadsheetManagementService
{
    private const string SpreadsheetType = "application/vnd.google-apps.spreadsheet";

    private const int DefaultSheetId = LabsSheet.Id;
    private const string DefaultSheetTitle = LabsSheet.Title;

    private const string UpdateTitle = "title";

    private static readonly Permission AnyoneViewerPermission = new() { Type = "anyone", Role = "reader" };

    private readonly DriveService _driveService;
    private readonly ILogger<SheetManagementService> _logger;

    private readonly SheetsService _sheetsService;
    private readonly ITablesParentsProvider _tablesParentsProvider;

    public SpreadsheetManagementService(
        SheetsService sheetsService,
        DriveService driveService,
        ITablesParentsProvider tablesParentsProvider,
        ILogger<SheetManagementService> logger)
    {
        _sheetsService = sheetsService;
        _driveService = driveService;
        _tablesParentsProvider = tablesParentsProvider;
        _logger = logger;
    }

    public async Task<string> CreateSpreadsheetAsync(string title, CancellationToken token)
    {
        var spreadsheetToCreate = new File
        {
            Parents = _tablesParentsProvider.GetParents(),
            MimeType = SpreadsheetType,
            Name = title
        };

        _logger.LogDebug("Create file {title} on Google drive.", title);

        File spreadsheetFile = await _driveService.Files
            .Create(spreadsheetToCreate)
            .ExecuteAsync(token);

        string spreadsheetId = spreadsheetFile.Id;

        await ConfigureDefaultSheetAsync(spreadsheetId, token);

        _logger.LogDebug("Update permission of file: {title}.", title);

        await _driveService.Permissions
            .Create(AnyoneViewerPermission, spreadsheetId)
            .ExecuteAsync(token);

        return spreadsheetId;
    }

    private async Task ConfigureDefaultSheetAsync(string spreadsheetId, CancellationToken token)
    {
        var defaultSheetProperties = new SheetProperties { SheetId = DefaultSheetId, Title = DefaultSheetTitle };

        var updatePropertiesRequest = new Request
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = defaultSheetProperties,
                Fields = UpdateTitle
            }
        };

        _logger.LogDebug("Configure default sheet for {spreadsheetId}.", spreadsheetId);

        await _sheetsService.ExecuteBatchUpdateAsync(spreadsheetId, updatePropertiesRequest, token);
    }
}