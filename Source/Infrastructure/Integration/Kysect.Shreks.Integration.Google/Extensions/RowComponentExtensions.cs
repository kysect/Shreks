using System.Drawing;
using FluentSpreadsheets;
using FluentSpreadsheets.Styles;
using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class RowComponentExtensions
{
    public static IRowComponent FilledWith(this IRowComponent row, int alpha, Color baseColor)
    {
        return row.FilledWith(Color.FromArgb(alpha, baseColor));
    }

    public static IRowComponent WithTopMediumBorder(this IRowComponent row)
    {
        return row.WithTopBorderType(BorderType.Medium);
    }

    public static IRowComponent WithBottomMediumBorder(this IRowComponent row)
    {
        return row.WithBottomBorderType(BorderType.Medium);
    }

    public static IRowComponent WithDefaultStyle(this IRowComponent row, int rowNumber, int maxRowNumber)
    {
        row = row.WithAlternatingColor(rowNumber);

        if (rowNumber is 0)
            row = row.WithTopMediumBorder();

        if (rowNumber == maxRowNumber - 1)
            row = row.WithBottomMediumBorder();

        return row;
    }

    private static IRowComponent WithAlternatingColor(this IRowComponent row, int rowNumber)
    {
        if (rowNumber % 2 is 0)
            row = row.FilledWith(Color.AliceBlue);

        return row;
    }
}