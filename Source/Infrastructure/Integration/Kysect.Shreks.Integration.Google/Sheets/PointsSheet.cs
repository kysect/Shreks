using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using Kysect.Shreks.Application.Abstractions.Google.Models;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class PointsSheet : ISheet<CoursePoints>
{
    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ISheetManagementService _sheetEditor;
    private readonly ISheetComponentFactory<CoursePoints> _sheetDataFactory;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    public PointsSheet(
        IUserFullNameFormatter userFullNameFormatter,
        ISheetManagementService sheetEditor,
        ISheetComponentFactory<CoursePoints> sheetDataFactory,
        IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _sheetEditor = sheetEditor;
        _sheetDataFactory = sheetDataFactory;
        _renderer = renderer;
    }

    public string Title => "Баллы";
    public int Id => 2;

    public async Task UpdateAsync(string spreadsheetId, CoursePoints points, CancellationToken token)
    {
        await _sheetEditor.CreateOrClearSheetAsync(spreadsheetId, this, token);

        CoursePoints sortedPoints = SortPoints(points);

        IComponent sheetData = _sheetDataFactory.Create(sortedPoints);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, Id, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }

    private CoursePoints SortPoints(CoursePoints points)
    {
        List<Assignment> sortedAssignments = points.Assignments
            .OrderBy(a => a.ShortName)
            .ToList();

        List<StudentPoints> sortedStudentPoints = points.StudentsPoints
            .OrderBy(p => p.Student.Group.Name)
            .ThenBy(p => _userFullNameFormatter.GetFullName(p.Student.User))
            .ToList();

        return new CoursePoints(sortedAssignments, sortedStudentPoints);
    }
}
