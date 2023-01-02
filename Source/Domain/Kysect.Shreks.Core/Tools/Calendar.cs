namespace Kysect.Shreks.Core.Tools;

public static class Calendar
{
    private static readonly TimeZoneInfo TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

    public static SpbDateTime CurrentDateTime =>
        new(DateTime.SpecifyKind(FromLocal(DateTime.Now).Value, DateTimeKind.Unspecified));

    public static DateOnly CurrentDate => DateOnly.FromDateTime(CurrentDateTime.Value);

    public static SpbDateTime FromLocal(DateTime dateTime)
    {
        return new(TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo));
    }

    public static SpbDateTime FromUtc(DateTime dateTime)
    {
        return new(TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo));
    }

    public static DateTime ToUtc(SpbDateTime dateTime)
    {
        return TimeZoneInfo.ConvertTime(dateTime.Value, TimeZoneInfo, TimeZoneInfo.Utc);
    }
}