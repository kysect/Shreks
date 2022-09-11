namespace Kysect.Shreks.Core.Tools;

public record struct SpbDateTime(DateTime Value)
{
    public static SpbDateTime FromDateOnly(DateOnly dateOnly)
    {
        return new SpbDateTime(dateOnly.ToDateTime(TimeOnly.MinValue));
    }
}