using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Indices;
using Kysect.Shreks.GoogleIntegration.Extensions;

namespace Kysect.Shreks.GoogleIntegration.Factories.Ranges;

public static class GridRangeFactory
{
    public static GridRange Create(
        int sheetId,
        SheetIndex startSheetIndex,
        SheetIndex endSheetIndex)
    {
        var gridRange = new GridRange
        {
            SheetId = sheetId
        };

        if (startSheetIndex.Row.IsNotNone())
        {
            gridRange.StartRowIndex = startSheetIndex.Row.Value - 1;
        }

        if (startSheetIndex.Column.IsNotNone())
        {
            gridRange.StartColumnIndex = startSheetIndex.Column.Value - 1;
        }

        if (endSheetIndex.Row.IsNotNone())
        {
            gridRange.EndRowIndex = endSheetIndex.Row.Value;
        }

        if (endSheetIndex.Column.IsNotNone())
        {
            gridRange.EndColumnIndex = endSheetIndex.Column.Value;
        }

        return gridRange;
    }
}