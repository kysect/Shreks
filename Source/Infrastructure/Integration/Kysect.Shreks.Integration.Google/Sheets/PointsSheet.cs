using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class PointsSheet : ISheet<int>
{
    public const string Title = "Баллы";

    private readonly ISheetManagementService _sheetEditor;
    private readonly ITable<int> _pointsTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    public PointsSheet(
        ISheetManagementService sheetEditor,
        ITable<int> pointsTable,
        IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _sheetEditor = sheetEditor;
        _pointsTable = pointsTable;
        _renderer = renderer;
    }

    public async Task UpdateAsync(string spreadsheetId, int studentsCount, CancellationToken token)
    {
        int sheetId = await _sheetEditor.CreateOrClearSheetAsync(spreadsheetId, Title, token);

        IComponent sheetData = _pointsTable.Render(studentsCount);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, sheetId, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }
}