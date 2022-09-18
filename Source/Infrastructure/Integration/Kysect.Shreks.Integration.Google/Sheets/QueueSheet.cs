using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class QueueSheet : ISheet<SubmissionsQueueDto>
{
    private readonly ISheetManagementService _sheetEditor;
    private readonly ITable<SubmissionsQueueDto> _queueTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

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
