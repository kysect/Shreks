namespace Kysect.Shreks.GoogleIntegration.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class GoogleSheetAttribute : Attribute
{
    public string Title { get; }
    public string HeaderRange { get; }
    public string DataRange { get; }

    public GoogleSheetAttribute(string title, string headerRange, string dataRange)
    {
        Title = title;
        HeaderRange = headerRange;
        DataRange = dataRange;
    }

    public void Deconstruct(out string title, out string headerRange, out string dataRange)
    {
        title = Title;
        headerRange = HeaderRange;
        dataRange = DataRange;
    }
}