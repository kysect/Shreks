using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google.Models;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using MediatR;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Segments;

public class TotalPointsSegment : SheetSegmentBase<CoursePoints, StudentPoints, Unit>
{
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public TotalPointsSegment(ICultureInfoProvider cultureInfoProvider)
    {
        _cultureInfoProvider = cultureInfoProvider;
    }

    protected override IComponent BuildHeader(CoursePoints data)
        => Label("Итог").WithDefaultStyle();

    protected override IComponent BuildRow(HeaderRowData<CoursePoints, StudentPoints> data, int rowIndex)
    {
        double totalPoints = data.RowData.Points.Sum(p => p.Points.Value);

        return Label(totalPoints.ToSheetPoints(_cultureInfoProvider.GetCultureInfo())).WithDefaultStyle();
    }
}