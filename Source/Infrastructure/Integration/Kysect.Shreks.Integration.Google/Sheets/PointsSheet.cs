using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.SheetBuilders;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Segments;
using Kysect.Shreks.Integration.Google.Tools;
using MediatR;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class PointsSheet : ISheet<Points>
{
    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ISpreadsheetIdProvider _spreadsheetIdProvider;
    private readonly ISheetController _sheetEditor;
    private readonly ISheetBuilder _sheetBuilder;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    private readonly ISheetSegment<Points, StudentPoints, Unit>[] _segments;

    public PointsSheet(
        IUserFullNameFormatter userFullNameFormatter,
        ISpreadsheetIdProvider spreadsheetIdProvider,
        ISheetController sheetEditor,
        ISheetBuilder sheetBuilder,
        IComponentRenderer<GoogleSheetRenderCommand> renderer,
        PointsStudentSegment studentSegment,
        AssignmentPointsSegment assignmentPointsSegment,
        TotalPointsSegment finalPointsSegment)
    {
        _userFullNameFormatter = userFullNameFormatter;
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

        Points sortedPoints = SortPoints(points);

        var sheetData = new SheetData<Points, StudentPoints, Unit>(sortedPoints, sortedPoints.StudentsPoints, Unit.Value);

        IComponent sheet = _sheetBuilder.Build(_segments, sheetData);

        var renderCommand = new GoogleSheetRenderCommand(_spreadsheetIdProvider.SpreadsheetId, Id, Title, sheet);
        await _renderer.RenderAsync(renderCommand, token);
    }

    private Points SortPoints(Points points)
    {
        List<Assignment> sortedAssignments = points.Assignments
            .OrderBy(a => a.ShortName)
            .ToList();

        List<StudentPoints> sortedStudentPoints = points.StudentsPoints
            .OrderBy(p => p.Student.Group.Name)
            .ThenBy(p => _userFullNameFormatter.GetFullName(p.Student))
            .ToList();

        return new Points(sortedAssignments, sortedStudentPoints);
    }
}
