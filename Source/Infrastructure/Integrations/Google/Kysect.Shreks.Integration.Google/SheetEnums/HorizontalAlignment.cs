namespace Kysect.Shreks.Integration.Google.SheetEnums;

public readonly struct HorizontalAlignment
{
    private readonly string _value;

    private HorizontalAlignment(string value)
        => _value = value;

    public static HorizontalAlignment Left => new("LEFT");
    public static HorizontalAlignment Center => new("CENTER");
    public static HorizontalAlignment Right => new("RIGHT");

    public static implicit operator string(HorizontalAlignment alignment)
        => alignment.ToString();

    public override string ToString()
        => _value;
}