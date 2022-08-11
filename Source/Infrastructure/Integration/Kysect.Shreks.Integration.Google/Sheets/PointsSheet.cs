using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.SheetBuilders;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.GoogleSheets;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Segments;
using Kysect.Shreks.Integration.Google.Tools;
using MediatR;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class PointsSheet : ISheet<Points>
{
    private readonly ISpreadsheetIdProvider _spreadsheetIdProvider;
    private readonly ISheetController _sheetEditor;
    private readonly ISheetBuilder _sheetBuilder;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    private readonly ISheetSegment<Points, StudentPoints, Unit>[] _segments;

    public PointsSheet(
        ISpreadsheetIdProvider spreadsheetIdProvider,
        ISheetController sheetEditor,
        ISheetBuilder sheetBuilder,
        IComponentRenderer<GoogleSheetRenderCommand> renderer,
        PointsStudentSegment studentSegment,
        AssignmentPointsSegment assignmentPointsSegment,
        TotalPointsSegment finalPointsSegment)
    {
        _spreadsheetIdProvider = spreadsheetIdProvider;
        _sheetEditor = sheetEditor;
        _sheetBuilder = sheetBuilder;
        _renderer = renderer;
        
        _segments = new ISheetSegment<Points, StudentPoints, Unit>[]
        {
            studentSegment,
            assignmentPointsSegment,
            finalPointsSegment
        };
    }

    public string Title => "Баллы";
    public int Id => 2;

    public async Task UpdateAsync(Points points, CancellationToken token)
    {
        await _sheetEditor.CreateOrClearSheetAsync(this, token);

        var sheetData = new SheetData<Points, StudentPoints, Unit>(points, points.StudentsPoints, Unit.Value);

        IComponent sheet = _sheetBuilder.Build(_segments, sheetData);

        var renderCommand = new GoogleSheetRenderCommand(_spreadsheetIdProvider.SpreadsheetId, Id, Title, sheet);
        await _renderer.RenderAsync(renderCommand, token);
    }
}
