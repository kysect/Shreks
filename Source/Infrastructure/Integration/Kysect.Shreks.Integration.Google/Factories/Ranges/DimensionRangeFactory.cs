using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Indices;
using Kysect.Centum.Sheets.Models;
using Kysect.Shreks.Integration.Google.Extensions;

namespace Kysect.Shreks.Integration.Google.Factories.Ranges;

public static class DimensionRangeFactory
{
    public static DimensionRange Create(
        int sheetId,
        SheetIndex startSheetIndex,
        SheetIndex endSheetIndex,
        Dimension dimension)
    {
        var dimensionRange = new DimensionRange
        {
            SheetId = sheetId,
            Dimension = dimension
        };

        if (dimensionRange.Dimension == Dimension.Columns)
        {
            if (startSheetIndex.Column.IsNotNone())
            {
                dimensionRange.StartIndex = startSheetIndex.Column.Value;
            }

            if (endSheetIndex.Column.IsNotNone())
            {
                dimensionRange.EndIndex = endSheetIndex.Column.Value;
            }
        }
        else
        {
            if (startSheetIndex.Row.IsNotNone())
            {
                dimensionRange.StartIndex = startSheetIndex.Column.Value;
            }

            if (endSheetIndex.Row.IsNotNone())
            {
                dimensionRange.EndIndex = endSheetIndex.Row.Value;
            }
        }

        return dimensionRange;
    }
}