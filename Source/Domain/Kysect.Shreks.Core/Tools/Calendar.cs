namespace Kysect.Shreks.Core.Tools;

public static class Calendar
{
    private static readonly TimeZoneInfo TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

    public static DateTime CurrentDateTime => DateTime.SpecifyKind(Convert(DateTime.Now), DateTimeKind.Local);

    public static DateOnly CurrentDate => DateOnly.FromDateTime(CurrentDateTime);

    public static DateTime Convert(DateTime dateTime)
        => TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo);
}