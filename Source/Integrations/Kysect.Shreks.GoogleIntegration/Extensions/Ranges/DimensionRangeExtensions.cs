using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Indices;
using Kysect.Centum.Sheets.Models;

namespace Kysect.Shreks.GoogleIntegration.Extensions.Ranges;

public static class DimensionRangeExtensions
{
    public static DimensionRange Fill(
        this DimensionRange dimensionRange,
        SheetIndex startSheetIndex,
        SheetIndex endSheetIndex,
        Dimension dimension)
    {
        dimensionRange.Dimension = dimension;

        if (dimensionRange.Dimension == Dimension.Columns)
        {
            if (startSheetIndex.Column != ColumnIndex.None)
            {
                dimensionRange.StartIndex = startSheetIndex.Column.Value;
            }
            
            if (endSheetIndex.Column != ColumnIndex.None)
            {
                dimensionRange.EndIndex = endSheetIndex.Column.Value;
            }
        }
        else
        {
            if (startSheetIndex.Row != RowIndex.None)
            {
                dimensionRange.StartIndex = startSheetIndex.Column.Value;
            }

            if (endSheetIndex.Row != RowIndex.None)
            {
                dimensionRange.EndIndex = endSheetIndex.Row.Value;
            }
        }

        return dimensionRange;
    }
}