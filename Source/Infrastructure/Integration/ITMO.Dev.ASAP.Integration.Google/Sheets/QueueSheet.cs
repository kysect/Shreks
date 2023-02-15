using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using ITMO.Dev.ASAP.Application.Abstractions.Google.Sheets;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Integration.Google.Tools;

namespace ITMO.Dev.ASAP.Integration.Google.Sheets;

public class QueueSheet : ISheet<SubmissionsQueueDto>
{
    private readonly ITable<SubmissionsQueueDto> _queueTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;
    private readonly ISheetManagementService _sheetEditor;

    public QueueSheet(
        ISheetManagementService sheetEditor,
        ITable<SubmissionsQueueDto> queueTable,
        IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _sheetEditor = sheetEditor;
        _queueTable = queueTable;
        _renderer = renderer;
    }

    public async Task UpdateAsync(string spreadsheetId, SubmissionsQueueDto model, CancellationToken token)
    {
        string title = model.GroupName;

        int sheetId = await _sheetEditor.CreateOrClearSheetAsync(spreadsheetId, title, token);

        IComponent sheetData = _queueTable.Render(model);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, sheetId, title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }
}