using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using MediatR;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Segments;

public class AssignmentDataSegment : SheetSegmentBase<Unit, QueueSubmissionDto, Unit>
{
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public AssignmentDataSegment(ICultureInfoProvider cultureInfoProvider)
    {
        _cultureInfoProvider = cultureInfoProvider;
    }

    protected override IComponent BuildHeader(Unit data)
    {
        return HStack
        (
            Label("Лабораторная работа")
                .WithColumnWidth(150)
                .WithDefaultStyle(),
            Label("Дата")
                .WithDefaultStyle(),
            Label("GitHub")
                .WithColumnWidth(400)
                .WithDefaultStyle()
        );
    }

    protected override IComponent BuildRow(HeaderRowData<Unit, QueueSubmissionDto> data, int rowIndex)
    {
        var submission = data.RowData.Submission;

        return HStack
        (
            Label(submission.AssignmentShortName).WithDefaultStyle(),
            Label(submission.SubmissionDate, _cultureInfoProvider.GetCultureInfo()).WithDefaultStyle(),
            Label(submission.Payload).WithDefaultStyle()
        );
    }
}