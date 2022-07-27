namespace Kysect.Shreks.GoogleIntegration.SheetEnums;

public readonly struct VerticalAlignment
{
    private readonly string _value;

    private VerticalAlignment(string value)
        => _value = value;

    public static VerticalAlignment Top => new("TOP");
    public static VerticalAlignment Middle => new("MIDDLE");
    public static VerticalAlignment Bottom => new("BOTTOM");

    public static implicit operator string(VerticalAlignment alignment)
        => alignment.ToString();

    public override string ToString()
        => _value;
}