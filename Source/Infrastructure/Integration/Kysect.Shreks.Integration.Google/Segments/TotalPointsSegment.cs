using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using MediatR;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Segments;

public class TotalPointsSegment : SheetSegmentBase<Points, StudentPoints, Unit>
{
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public TotalPointsSegment(ICultureInfoProvider cultureInfoProvider)
    {
        _cultureInfoProvider = cultureInfoProvider;
    }

    protected override IComponent BuildHeader(Points data)
        => Label("Итог").WithDefaultStyle();

    protected override IComponent BuildRow(HeaderRowData<Points, StudentPoints> data, int rowIndex)
    {
        double totalPoints = data.RowData.Points.Sum(p => p.Points);

        return Label(totalPoints.ToSheetPoints(_cultureInfoProvider.CultureInfo)).WithDefaultStyle();
    }
}