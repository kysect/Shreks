using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using ITMO.Dev.ASAP.Application.Abstractions.Google.Sheets;
using ITMO.Dev.ASAP.Integration.Google.Models;
using ITMO.Dev.ASAP.Integration.Google.Tools;

namespace ITMO.Dev.ASAP.Integration.Google.Sheets;

public class PointsSheet : ISheet<CourseStudentsDto>
{
    public const string Title = "Баллы";
    private readonly ITable<CourseStudentsDto> _pointsTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    private readonly ISheetManagementService _sheetEditor;

    public PointsSheet(
        ISheetManagementService sheetEditor,
        ITable<CourseStudentsDto> pointsTable,
        IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _sheetEditor = sheetEditor;
        _pointsTable = pointsTable;
        _renderer = renderer;
    }

    public async Task UpdateAsync(string spreadsheetId, CourseStudentsDto model, CancellationToken token)
    {
        int sheetId = await _sheetEditor.CreateSheetAsync(spreadsheetId, Title, token);

        IComponent sheetData = _pointsTable.Render(model);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, sheetId, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }
}