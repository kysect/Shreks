using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using MediatR;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Segments;

public class TotalPointsSegment : SheetSegmentBase<CoursePointsDto, StudentPointsDto, Unit>
{
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public TotalPointsSegment(ICultureInfoProvider cultureInfoProvider)
    {
        _cultureInfoProvider = cultureInfoProvider;
    }

    protected override IComponent BuildHeader(CoursePointsDto data)
        => Label("Итог").WithDefaultStyle();

    protected override IComponent BuildRow(HeaderRowData<CoursePointsDto, StudentPointsDto> data, int rowIndex)
    {
        double totalPoints = data.RowData
            .Points
            .Where(p => p.Points.HasValue)
            .Sum(p => p.Points.GetValueOrDefault());

        return Label(totalPoints.ToSheetPoints(_cultureInfoProvider.GetCultureInfo())).WithDefaultStyle();
    }
}