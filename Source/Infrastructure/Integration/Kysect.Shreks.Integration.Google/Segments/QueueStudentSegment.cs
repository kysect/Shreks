using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Factories;
using MediatR;

namespace Kysect.Shreks.Integration.Google.Segments;

public class QueueStudentSegment : SheetSegmentBase<Unit, Submission, Unit>
{
    private readonly IStudentComponentFactory _studentComponentFactory;

    public QueueStudentSegment(IStudentComponentFactory studentComponentFactory)
    {
        _studentComponentFactory = studentComponentFactory;
    }

    protected override IComponent BuildHeader(Unit data)
        => _studentComponentFactory.BuildHeader();

    protected override IComponent BuildRow(HeaderRowData<Unit, Submission> data, int rowIndex)
        => _studentComponentFactory.BuildRow(data.RowData.Student);
}