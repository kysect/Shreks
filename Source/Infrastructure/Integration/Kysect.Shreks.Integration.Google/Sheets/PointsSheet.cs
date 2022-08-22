using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class PointsSheet : ISheet<CoursePointsDto>
{
    private const string Title = "Баллы";

    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ISheetManagementService _sheetEditor;
    private readonly ISheetComponentFactory<CoursePointsDto> _sheetDataFactory;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    public PointsSheet(
        IUserFullNameFormatter userFullNameFormatter,
        ISheetManagementService sheetEditor,
        ISheetComponentFactory<CoursePointsDto> sheetDataFactory,
        IComponentRenderer<GoogleSheetRenderCommand> renderer)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _sheetEditor = sheetEditor;
        _sheetDataFactory = sheetDataFactory;
        _renderer = renderer;
    }

    public async Task UpdateAsync(string spreadsheetId, CoursePointsDto points, CancellationToken token)
    {
        var sheetId = await _sheetEditor.CreateOrClearSheetAsync(spreadsheetId, Title, token);

        CoursePointsDto sortedPoints = SortPoints(points);

        IComponent sheetData = _sheetDataFactory.Create(sortedPoints);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, sheetId, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);
    }

    private CoursePointsDto SortPoints(CoursePointsDto points)
    {
        List<AssignmentDto> sortedAssignments = points.Assignments
            .OrderBy(a => a.ShortName)
            .ToList();

        List<StudentPointsDto> sortedStudentPoints = points.StudentsPoints
            .OrderBy(p => p.Student.GroupName)
            .ThenBy(p => _userFullNameFormatter.GetFullName(p.Student.User))
            .ToList();

        return new CoursePointsDto(sortedAssignments, sortedStudentPoints);
    }
}
