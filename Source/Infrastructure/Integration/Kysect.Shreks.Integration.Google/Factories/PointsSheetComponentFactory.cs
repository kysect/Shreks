using FluentSpreadsheets;
using FluentSpreadsheets.SheetBuilders;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Segments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Factories;

public class PointsSheetComponentFactory : ISheetComponentFactory<Points>
{
    private readonly ISheetBuilder _sheetBuilder;

    private readonly ISheetSegment<Points, StudentPoints, Unit>[] _segments;

    public PointsSheetComponentFactory(
        ISheetBuilder sheetBuilder,
        IServiceProvider serviceProvider)
    {
        _sheetBuilder = sheetBuilder;

        _segments = new ISheetSegment<Points, StudentPoints, Unit>[]
        {
            ActivatorUtilities.CreateInstance<PointsStudentSegment>(serviceProvider),
            ActivatorUtilities.CreateInstance<AssignmentPointsSegment>(serviceProvider),
            ActivatorUtilities.CreateInstance<TotalPointsSegment>(serviceProvider)
        };
    }

    public IComponent Create(Points points)
    {
        var sheetData = new SheetData<Points, StudentPoints, Unit>(points, points.StudentsPoints, Unit.Value);
        return _sheetBuilder.Build(_segments, sheetData);
    }
}