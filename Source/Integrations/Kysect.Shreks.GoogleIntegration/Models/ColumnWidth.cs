using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Models;

namespace Kysect.Shreks.GoogleIntegration.Models;

public readonly record struct ColumnWidth(int ColumnIndex, int Width)
{
    public DimensionRange GetDimensionRange(int sheetId)
    {
        return new DimensionRange
        {
            StartIndex = ColumnIndex,
            EndIndex = ColumnIndex + 1,
            Dimension = Dimension.Columns,
            SheetId = sheetId
        };
    }
}