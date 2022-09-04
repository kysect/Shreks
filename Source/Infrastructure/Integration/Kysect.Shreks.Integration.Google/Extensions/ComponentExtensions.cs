using System.Drawing;
using FluentSpreadsheets;
using FluentSpreadsheets.Styles;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class ComponentExtensions
{
    public static IComponent WithDefaultStyle(this IComponent component)
    {
        return component
            .WithDefaultBorders()
            .WithCenterAlignment();
    }

    public static IComponent WithDefaultBorders(this IComponent component)
    {
        return component
            .WithTrailingBorder(BorderType.Thin, Color.Black)
            .WithBottomBorder(BorderType.Thin, Color.Black);
    }

    public static IComponent WithCenterAlignment(this IComponent component)
        => component.WithContentAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
}