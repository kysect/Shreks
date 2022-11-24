using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class LabsSheet : ISheet<SubjectCoursePointsDto>
{
    public const int Id = 0;
    public const string Title = "Лабораторные";

    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ISheetManagementService _sheetEditor;
    private readonly ITable<SubjectCoursePointsDto> _pointsTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;
    private readonly ISheet<CourseStudentsDto> _pointsSheet;

    public LabsSheet(
        IUserFullNameFormatter userFullNameFormatter,
        ISheetManagementService sheetEditor,
        ITable<SubjectCoursePointsDto> pointsTable,
        IComponentRenderer<GoogleSheetRenderCommand> renderer,
        ISheet<CourseStudentsDto> pointsSheet)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _sheetEditor = sheetEditor;
        _pointsTable = pointsTable;
        _renderer = renderer;
        _pointsSheet = pointsSheet;
    }

    public async Task UpdateAsync(string spreadsheetId, SubjectCoursePointsDto model, CancellationToken token)
    {
        SubjectCoursePointsDto sortedPoints = SortPoints(model);
        int sheetId = await _sheetEditor.CreateOrClearSheetAsync(spreadsheetId, Title, token);

        IComponent sheetData = _pointsTable.Render(sortedPoints);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, sheetId, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);

        bool labsSheetExist = await _sheetEditor.CheckIfExists(spreadsheetId, PointsSheet.Title, token);
        if (!labsSheetExist)
        {
            var courseStudents = new CourseStudentsDto(model.StudentsPoints);
            await _pointsSheet.UpdateAsync(spreadsheetId, courseStudents, token);
        }
    }

    private SubjectCoursePointsDto SortPoints(SubjectCoursePointsDto points)
    {
        var sortedAssignments = points.Assignments
            .OrderBy(a => a.Order)
            .ToList();

        var sortedStudentPoints = points.StudentsPoints
            .OrderBy(p => p.Student.GroupName)
            .ThenBy(p => _userFullNameFormatter.GetFullName(p.Student.User))
            .ToList();

        return new SubjectCoursePointsDto(sortedAssignments, sortedStudentPoints);
    }
}
