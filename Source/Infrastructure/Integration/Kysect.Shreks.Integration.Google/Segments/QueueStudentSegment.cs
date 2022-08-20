using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Factories;
using MediatR;

namespace Kysect.Shreks.Integration.Google.Segments;

public class QueueStudentSegment : SheetSegmentBase<Unit, QueueSubmissionDto, Unit>
{
    private readonly IStudentComponentFactory _studentComponentFactory;

    public QueueStudentSegment(IStudentComponentFactory studentComponentFactory)
    {
        _studentComponentFactory = studentComponentFactory;
    }

    protected override IComponent BuildHeader(Unit data)
        => _studentComponentFactory.BuildHeader();

    protected override IComponent BuildRow(HeaderRowData<Unit, QueueSubmissionDto> data, int rowIndex)
        => _studentComponentFactory.BuildRow(data.RowData.Student);
}