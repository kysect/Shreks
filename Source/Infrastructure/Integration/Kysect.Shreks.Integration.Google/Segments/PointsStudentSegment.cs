using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Factories;
using MediatR;

namespace Kysect.Shreks.Integration.Google.Segments;

public class PointsStudentSegment : SheetSegmentBase<CoursePointsDto, StudentPointsDto, Unit>
{
    private readonly IStudentComponentFactory _studentComponentFactory;

    public PointsStudentSegment(IStudentComponentFactory studentComponentFactory)
    {
        _studentComponentFactory = studentComponentFactory;
    }

    protected override IComponent BuildHeader(CoursePointsDto data)
        => _studentComponentFactory.BuildHeader();

    protected override IComponent BuildRow(HeaderRowData<CoursePointsDto, StudentPointsDto> data, int rowIndex)
        => _studentComponentFactory.BuildRow(data.RowData.Student);
}