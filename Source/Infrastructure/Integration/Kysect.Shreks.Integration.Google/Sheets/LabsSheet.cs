using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class LabsSheet : ISheet<CoursePointsDto>
{
    public const int Id = 0;
    public const string Title = "Лабораторные";

    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ISheetManagementService _sheetEditor;
    private readonly ITable<CoursePointsDto> _pointsTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;
    private readonly ISheet<int> _pointsSheet;

    public LabsSheet(
        IUserFullNameFormatter userFullNameFormatter,
        ISheetManagementService sheetEditor,
        ITable<CoursePointsDto> pointsTable,
        IComponentRenderer<GoogleSheetRenderCommand> renderer,
        ISheet<int> pointsSheet)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _sheetEditor = sheetEditor;
        _pointsTable = pointsTable;
        _renderer = renderer;
        _pointsSheet = pointsSheet;
    }

    public async Task UpdateAsync(string spreadsheetId, CoursePointsDto points, CancellationToken token)
    {
        CoursePointsDto sortedPoints = SortPoints(points);

        IComponent sheetData = _pointsTable.Render(sortedPoints);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, Id, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);

        bool labsSheetExist = await _sheetEditor.CheckIfExists(spreadsheetId, PointsSheet.Title, token);
        if (!labsSheetExist)
            await _pointsSheet.UpdateAsync(spreadsheetId, points.StudentsPoints.Count, token);
    }

    private CoursePointsDto SortPoints(CoursePointsDto points)
    {
        List<AssignmentDto> sortedAssignments = points.Assignments
            .OrderBy(a => a.Order)
            .ToList();

        List<StudentPointsDto> sortedStudentPoints = points.StudentsPoints
            .OrderBy(p => p.Student.GroupName)
            .ThenBy(p => _userFullNameFormatter.GetFullName(p.Student.User))
            .ToList();

        return new CoursePointsDto(sortedAssignments, sortedStudentPoints);
    }
}
