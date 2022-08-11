using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.GoogleSheets;
using Kysect.Shreks.Integration.Google.Segments.Factories;
using MediatR;

namespace Kysect.Shreks.Integration.Google.Segments;

public class PointsStudentSegment : SheetSegmentBase<Points, StudentPoints, Unit>
{
    private readonly IStudentComponentFactory _studentComponentFactory;

    public PointsStudentSegment(IStudentComponentFactory studentComponentFactory)
    {
        _studentComponentFactory = studentComponentFactory;
    }

    protected override IComponent BuildHeader(Points data)
        => _studentComponentFactory.BuildHeader();

    protected override IComponent BuildRow(HeaderRowData<Points, StudentPoints> data, int rowIndex)
        => _studentComponentFactory.BuildRow(data.RowData.Student);
}