using System.Globalization;
using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using MediatR;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Segments;

public class AssignmentPointsSegment : PrototypeSheetSegmentBase<Points, StudentPoints, Unit, Assignment, AssignmentPoints?>
{
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public AssignmentPointsSegment(ICultureInfoProvider cultureInfoProvider)
    {
        _cultureInfoProvider = cultureInfoProvider;
    }

    protected override IComponent BuildHeader(Assignment data)
    {
        return VStack
        (
            Label(data.ShortName).WithDefaultStyle(),
            HStack
            (
                Label("Балл").WithDefaultStyle(),
                Label("Дата").WithDefaultStyle()
            )
        );
    }

    protected override IComponent BuildRow(HeaderRowData<Assignment, AssignmentPoints?> data, int rowIndex)
    {
        CultureInfo currentCulture = _cultureInfoProvider.CultureInfo;
        AssignmentPoints? assignmentPoints = data.RowData;

        IComponent pointsComponent = assignmentPoints is null
            ? Empty()
            : Label(assignmentPoints.Points.ToSheetPoints(currentCulture));

        IComponent dateComponent = assignmentPoints is null
            ? Empty()
            : Label(assignmentPoints.Date, currentCulture);

        return HStack
        (
            pointsComponent.WithDefaultStyle(),
            dateComponent.WithDefaultStyle()
        );
    }

    protected override IEnumerable<Assignment> SelectHeaderData(Points data)
        => data.Assignments;

    protected override AssignmentPoints? SelectRowData(HeaderRowData<Assignment, StudentPoints> data)
        => data.RowData.Points.FirstOrDefault(p => p.Assignment.Equals(data.HeaderData));
}