using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using Kysect.Shreks.Application.Abstractions.Google.Models;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class QueueSheet : ISheet<SubmissionsQueue>
{
    private readonly ISheetManagementService _sheetEditor;
    private readonly ISheetComponentFactory<SubmissionsQueue> _sheetDataFactory;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    public QueueSheet(
        ISheetManagementService sheetEditor,
        ISheetComponentFactory<SubmissionsQueue> sheetDataFactory,
        IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _sheetEditor = sheetEditor;
        _sheetDataFactory = sheetDataFactory;
        _renderer = renderer;
    }

    public string Title => "Очередь";
    public int Id => 1;

    public async Task UpdateAsync(string spreadsheetId, SubmissionsQueue queue, CancellationToken token)
    {
        await _sheetEditor.CreateOrClearSheetAsync(spreadsheetId, this, token);

        IComponent sheetData = _sheetDataFactory.Create(queue);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, Id, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }
}
