using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class PointsSheet : ISheet<Unit>
{
    public const int Id = 0;
    public const string Title = "Баллы";
    
    private readonly ITable<Unit> _queueTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    public PointsSheet(ITable<Unit> queueTable, IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _queueTable = queueTable;
        _renderer = renderer;
    }

    public async Task UpdateAsync(string spreadsheetId, Unit _, CancellationToken token)
    {
        IComponent sheetData = _queueTable.Render(Unit.Value);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, Id, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }
}