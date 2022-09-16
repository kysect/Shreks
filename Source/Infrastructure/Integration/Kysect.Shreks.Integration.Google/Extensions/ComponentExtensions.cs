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
            .WithTrailingBorderType(BorderType.Thin)
            .WithBottomBorderType(BorderType.Thin);
    }

    public static IComponent WithSideMediumBorder(this IComponent component)
    {
        return component
            .WithLeadingMediumBorder()
            .WithTrailingMediumBorder();
    }

    public static IComponent WithLeadingMediumBorder(this IComponent component)
        => component.WithLeadingBorderType(BorderType.Medium);

    public static IComponent WithTrailingMediumBorder(this IComponent component)
        => component.WithTrailingBorderType(BorderType.Medium);

    public static IComponent WithBottomMediumBorder(this IComponent component)
        => component.WithBottomBorderType(BorderType.Medium);

    public static IComponent WithCenterAlignment(this IComponent component)
        => component.WithContentAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
}