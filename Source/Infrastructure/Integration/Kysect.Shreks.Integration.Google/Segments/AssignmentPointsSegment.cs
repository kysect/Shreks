using System.Globalization;
using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using MediatR;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Segments;

public class AssignmentPointsSegment 
    : PrototypeSheetSegmentBase<CoursePointsDto, StudentPointsDto, Unit, AssignmentDto, AssignmentPointsDto?>
{
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public AssignmentPointsSegment(ICultureInfoProvider cultureInfoProvider)
    {
        _cultureInfoProvider = cultureInfoProvider;
    }

    protected override IComponent BuildHeader(AssignmentDto data)
    {
        ArgumentNullException.ThrowIfNull(data);

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

    protected override IComponent BuildRow(HeaderRowData<AssignmentDto, AssignmentPointsDto?> data, int rowIndex)
    {
        CultureInfo currentCulture = _cultureInfoProvider.GetCultureInfo();
        AssignmentPointsDto? assignmentPoints = data.RowData;

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

    protected override IEnumerable<AssignmentDto> SelectHeaderData(CoursePointsDto data)
        => data.Assignments;

    protected override AssignmentPointsDto? SelectRowData(HeaderRowData<AssignmentDto, StudentPointsDto> data)
        => data.RowData.Points.FirstOrDefault(p => p.AssignmentId.Equals(data.HeaderData.Id));
}