namespace Kysect.Shreks.Core.Tools;

public record struct SpbDateTime(DateTime Value)
{
    public static SpbDateTime FromDateOnly(DateOnly dateOnly)
    {
        return new SpbDateTime(dateOnly.ToDateTime(TimeOnly.MinValue));
    }

    public override string ToString()
    {
        return ToUserFriendlyString();
    }

    public string ToUserFriendlyString()
    {
        return Value.ToString("dd.MM.yyyy");
    }
}