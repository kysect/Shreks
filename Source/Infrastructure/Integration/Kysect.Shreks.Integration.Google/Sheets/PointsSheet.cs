using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Google.Sheets;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class PointsSheet : ISheet<CourseStudentsDto>
{
    public const string Title = "Баллы";

    private readonly ISheetManagementService _sheetEditor;
    private readonly ITable<CourseStudentsDto> _pointsTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

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