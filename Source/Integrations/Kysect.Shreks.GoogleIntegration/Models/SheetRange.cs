using System.Text.RegularExpressions;
using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Indices;
using Kysect.Centum.Sheets.Models;
using Kysect.Shreks.GoogleIntegration.Extensions.Ranges;

namespace Kysect.Shreks.GoogleIntegration.Models;

public class SheetRange
{
    private static readonly Regex AlphabeticRegex = new("^[a-zA-Z0-9]*$", RegexOptions.Compiled);

    public SheetRange(string title, int id, string range)
    {
        string titleReference = AlphabeticRegex.IsMatch(title)
            ? title
            : $"'{title}'";

        Range = $"{titleReference}!{range}";

        string[] rangeParts = range.Split(':');
        if (rangeParts.Length is not 2)
            throw new ArgumentException("Range must contain two indices");

        var startSheetIndex = new SheetIndex(rangeParts.First());
        var endSheetIndex = new SheetIndex(rangeParts.Last());

        ColumnDimensionRange = new DimensionRange { SheetId = id }
            .Fill(startSheetIndex, endSheetIndex, Dimension.Columns);

        if (endSheetIndex.Column != ColumnIndex.None)
        {
            ColumnDimensionRange.EndIndex = endSheetIndex.Column.Value;
        }

        GridRange = new GridRange { SheetId = id }
            .Fill(startSheetIndex, endSheetIndex);

        FrozenRowProperties = new SheetProperties
        {
            SheetId = id,
            GridProperties = new GridProperties
            {
                FrozenRowCount = endSheetIndex.Row.Value
            }
        };
    }

    public string Range { get; }
    public DimensionRange ColumnDimensionRange { get; }
    public GridRange GridRange { get; }
    
    public SheetProperties FrozenRowProperties { get; }
}