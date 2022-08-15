using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Extensions;
using MediatR;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Segments;

public class AssignmentDataSegment : SheetSegmentBase<Unit, Submission, Unit>
{
    protected override IComponent BuildHeader(Unit data)
    {
        return HStack
        (
            Label("Лабораторная работа")
                .WithColumnWidth(150)
                .WithDefaultStyle(),
            Label("GitHub")
                .WithColumnWidth(400)
                .WithDefaultStyle()
        );
    }

    protected override IComponent BuildRow(HeaderRowData<Unit, Submission> data, int rowIndex)
    {
        Submission submission = data.RowData;

        return HStack
        (
            Label(submission.Assignment.ShortName).WithDefaultStyle(),
            Label(submission.Payload).WithDefaultStyle()
        );
    }
}