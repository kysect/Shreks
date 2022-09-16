using System.Drawing;
using FluentSpreadsheets;
using FluentSpreadsheets.Styles;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class RowComponentExtensions
{
    public static IRowComponent FilledWith(this IRowComponent component, int alpha, Color baseColor)
        => component.FilledWith(Color.FromArgb(alpha, baseColor));

    public static IRowComponent WithTopMediumBorder(this IRowComponent component)
        => component.WithTopBorderType(BorderType.Medium);

    public static IRowComponent WithBottomMediumBorder(this IRowComponent component)
        => component.WithBottomBorderType(BorderType.Medium);
}