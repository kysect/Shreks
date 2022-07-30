using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Indices;
using Kysect.Centum.Sheets.Models;

namespace Kysect.Shreks.GoogleIntegration.Factories;

public interface IRangeFactory
{
    public DimensionRange GetDimensionRange(
        SheetIndex startSheetIndex,
        SheetIndex endSheetIndex,
        Dimension dimension);

    public GridRange GetGridRange(SheetIndex startSheetIndex, SheetIndex endSheetIndex);
}