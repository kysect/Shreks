using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Factories;
using MediatR;

namespace Kysect.Shreks.Integration.Google.Segments;

public class PointsStudentSegment : SheetSegmentBase<CoursePoints, StudentPoints, Unit>
{
    private readonly IStudentComponentFactory _studentComponentFactory;

    public PointsStudentSegment(IStudentComponentFactory studentComponentFactory)
    {
        _studentComponentFactory = studentComponentFactory;
    }

    protected override IComponent BuildHeader(CoursePoints data)
        => _studentComponentFactory.BuildHeader();

    protected override IComponent BuildRow(HeaderRowData<CoursePoints, StudentPoints> data, int rowIndex)
        => _studentComponentFactory.BuildRow(data.RowData.Student);
}