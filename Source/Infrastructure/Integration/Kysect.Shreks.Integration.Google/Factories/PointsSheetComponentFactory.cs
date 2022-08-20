using FluentSpreadsheets;
using FluentSpreadsheets.SheetBuilders;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Segments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Factories;

public class PointsSheetComponentFactory : ISheetComponentFactory<CoursePointsDto>
{
    private readonly ISheetBuilder _sheetBuilder;

    private readonly ISheetSegment<CoursePointsDto, StudentPointsDto, Unit>[] _segments;

    public PointsSheetComponentFactory(
        ISheetBuilder sheetBuilder,
        IServiceProvider serviceProvider)
    {
        _sheetBuilder = sheetBuilder;

        _segments = new ISheetSegment<CoursePointsDto, StudentPointsDto, Unit>[]
        {
            ActivatorUtilities.CreateInstance<PointsStudentSegment>(serviceProvider),
            ActivatorUtilities.CreateInstance<AssignmentPointsSegment>(serviceProvider),
            ActivatorUtilities.CreateInstance<TotalPointsSegment>(serviceProvider)
        };
    }

    public IComponent Create(CoursePointsDto points)
    {
        var sheetData = new SheetData<CoursePointsDto, StudentPointsDto, Unit>(points, points.StudentsPoints, Unit.Value);
        return _sheetBuilder.Build(_segments, sheetData);
    }
}