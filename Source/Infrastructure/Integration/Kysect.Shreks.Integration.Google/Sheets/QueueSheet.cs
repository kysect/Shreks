using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class QueueSheet : ISheet<Queue>
{
    private readonly ISpreadsheetIdProvider _spreadsheetIdProvider;
    private readonly ISheetManagementService _sheetEditor;
    private readonly ISheetComponentFactory<Queue> _sheetDataFactory;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    public QueueSheet(
        ISpreadsheetIdProvider spreadsheetIdProvider,
        ISheetManagementService sheetEditor,
        ISheetComponentFactory<Queue> sheetDataFactory,
        IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _spreadsheetIdProvider = spreadsheetIdProvider;
        _sheetEditor = sheetEditor;
        _sheetDataFactory = sheetDataFactory;
        _renderer = renderer;
    }

    public string Title => "Очередь";
    public int Id => 1;

    public async Task UpdateAsync(Queue queue, CancellationToken token)
    {
        await _sheetEditor.CreateOrClearSheetAsync(this, token);

        IComponent sheetData = _sheetDataFactory.Create(queue);
        var renderCommand = new GoogleSheetRenderCommand(_spreadsheetIdProvider.SpreadsheetId, Id, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }
}
