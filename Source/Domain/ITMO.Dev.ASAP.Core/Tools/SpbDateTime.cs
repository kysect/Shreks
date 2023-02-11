namespace ITMO.Dev.ASAP.Core.Tools;

public readonly record struct SpbDateTime(DateTime Value)
{
    public static SpbDateTime FromDateOnly(DateOnly dateOnly)
    {
        return new SpbDateTime(dateOnly.ToDateTime(TimeOnly.MinValue));
    }

    public DateOnly AsDateOnly()
    {
        return new DateOnly(Value.Year, Value.Month, Value.Day);
    }

    public override string ToString()
    {
        return Value.ToString("dd.MM.yyyy");
    }
}