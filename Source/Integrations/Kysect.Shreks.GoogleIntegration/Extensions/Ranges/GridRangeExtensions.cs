using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Indices;

namespace Kysect.Shreks.GoogleIntegration.Extensions.Ranges;

public static class GridRangeExtensions
{
    public static GridRange Fill(
        this GridRange gridRange,
        SheetIndex startSheetIndex,
        SheetIndex endSheetIndex)
    {
        if (startSheetIndex.Row != RowIndex.None)
        {
            gridRange.StartRowIndex = startSheetIndex.Row.Value - 1;
        }

        if (startSheetIndex.Column != ColumnIndex.None )
        {
            gridRange.StartColumnIndex = startSheetIndex.Column.Value - 1;
        }

        if (endSheetIndex.Row != RowIndex.None)
        {
            gridRange.EndRowIndex = endSheetIndex.Row.Value;
        }

        if (endSheetIndex.Column != ColumnIndex.None)
        {
            gridRange.EndColumnIndex = endSheetIndex.Column.Value;
        }

        return gridRange;
    }
}