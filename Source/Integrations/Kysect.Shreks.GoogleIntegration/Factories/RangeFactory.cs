using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Indices;
using Kysect.Centum.Sheets.Models;
using Kysect.Shreks.GoogleIntegration.Extensions;

namespace Kysect.Shreks.GoogleIntegration.Factories;

public class RangeFactory
{
    private readonly int _sheetId;

    public RangeFactory(int sheetId)
    {
        _sheetId = sheetId;
    }

    public DimensionRange GetDimensionRange(
        SheetIndex startSheetIndex,
        SheetIndex endSheetIndex,
        Dimension dimension)
    {
        var dimensionRange = new DimensionRange
        {
            SheetId = _sheetId,
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

    public GridRange GetGridRange(
        SheetIndex startSheetIndex,
        SheetIndex endSheetIndex)
    {
        var gridRange = new GridRange
        {
            SheetId = _sheetId
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