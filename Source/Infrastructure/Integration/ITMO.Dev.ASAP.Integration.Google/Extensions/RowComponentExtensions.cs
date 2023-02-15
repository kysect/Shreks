using FluentSpreadsheets;
using FluentSpreadsheets.Styles;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Dto.Users;
using System.Drawing;

namespace ITMO.Dev.ASAP.Integration.Google.Extensions;

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

    public static IRowComponent WithGroupSeparators(
        this IRowComponent row,
        int rowNumber,
        IReadOnlyList<StudentPointsDto> studentPoints)
    {
        ArgumentNullException.ThrowIfNull(studentPoints);

        if (rowNumber is 0)
            return row;

        StudentDto student1 = studentPoints[rowNumber].Student;
        StudentDto student2 = studentPoints[rowNumber - 1].Student;

        if (student1.GroupName != student2.GroupName)
            row = row.WithTopMediumBorder();

        return row;
    }

    private static IRowComponent WithAlternatingColor(this IRowComponent row, int rowNumber)
    {
        return (rowNumber % 2) switch
        {
            0 => row.FilledWith(Color.AliceBlue),
            _ => row.FilledWith(Color.Transparent),
        };
    }
}