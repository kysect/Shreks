using FluentSpreadsheets;
using FluentSpreadsheets.SheetBuilders;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Segments;
using MediatR;

namespace Kysect.Shreks.Integration.Google.Factories;

public class PointsSheetDataFactory : ISheetDataFactory<Points>
{
    private readonly ISheetBuilder _sheetBuilder;

    private readonly ISheetSegment<Points, StudentPoints, Unit>[] _segments;

    public PointsSheetDataFactory(
        ISheetBuilder sheetBuilder,
        PointsStudentSegment studentSegment,
        AssignmentPointsSegment assignmentPointsSegment,
        TotalPointsSegment finalPointsSegment)
    {
        _sheetBuilder = sheetBuilder;

        _segments = new ISheetSegment<Points, StudentPoints, Unit>[]
        {
            studentSegment,
            assignmentPointsSegment,
            finalPointsSegment
        };
    }

    public IComponent Create(Points points)
    {
        var sheetData = new SheetData<Points, StudentPoints, Unit>(points, points.StudentsPoints, Unit.Value);
        return _sheetBuilder.Build(_segments, sheetData);
    }
}