using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Mapping.Mappings;

public static class PointsMapping
{
    public static double AsDto(this Points points)
        => points.Value;

    public static Points AsPoints(this double points)
        => new Points(points);

    public static double? AsDto(this Points? points)
        => points?.Value;

    public static Points? AsPoints(this double? points)
        => points == null ? null : new Points(points.Value);
}