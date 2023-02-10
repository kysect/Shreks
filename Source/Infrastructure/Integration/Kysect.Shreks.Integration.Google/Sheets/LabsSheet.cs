﻿using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Google.Sheets;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class LabsSheet : ISheet<SubjectCoursePointsDto>
{
    public const int Id = 0;
    public const string Title = "Лабораторные";
    private readonly ISheet<CourseStudentsDto> _pointsSheet;
    private readonly ITable<SubjectCoursePointsDto> _pointsTable;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;
    private readonly ISheetManagementService _sheetEditor;

    public LabsSheet(
        ISheetManagementService sheetEditor,
        ITable<SubjectCoursePointsDto> pointsTable,
        IComponentRenderer<GoogleSheetRenderCommand> renderer,
        ISheet<CourseStudentsDto> pointsSheet)
    {
        _sheetEditor = sheetEditor;
        _pointsTable = pointsTable;
        _renderer = renderer;
        _pointsSheet = pointsSheet;
    }

    public async Task UpdateAsync(string spreadsheetId, SubjectCoursePointsDto model, CancellationToken token)
    {
        int sheetId = await _sheetEditor.CreateOrClearSheetAsync(spreadsheetId, Title, token);

        IComponent sheetData = _pointsTable.Render(model);
        var renderCommand = new GoogleSheetRenderCommand(spreadsheetId, sheetId, Title, sheetData);
        await _renderer.RenderAsync(renderCommand, token);

        bool pointsSheetExists = await _sheetEditor.CheckIfExists(spreadsheetId, PointsSheet.Title, token);

        if (pointsSheetExists is false)
        {
            var courseStudents = new CourseStudentsDto(model.StudentsPoints);
            await _pointsSheet.UpdateAsync(spreadsheetId, courseStudents, token);
        }
    }
}