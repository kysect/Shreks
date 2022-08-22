using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class QueueSheet : ISheet<SubmissionsQueueDto>
{
    private readonly ISheetManagementService _sheetEditor;
    private readonly ISheetComponentFactory<SubmissionsQueueDto> _sheetDataFactory;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    public QueueSheet(
        ISheetManagementService sheetEditor,
        ISheetComponentFactory<SubmissionsQueueDto> sheetDataFactory,
        IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _sheetEditor = sheetEditor;
        _sheetDataFactory = sheetDataFactory;
        _renderer = renderer;
    }

    public async Task UpdateAsync(string spreadsheetId, SubmissionsQueueDto queue, CancellationToken token)
    {
        var title = queue.GroupName;

        var sheetId = await _sheetEditor.CreateOrClearSheetAsync(spreadsheetId, title, token);

        IComponent sheetData = _sheetDataFactory.Create(queue);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, sheetId, title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }
}
