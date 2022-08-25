namespace Kysect.Shreks.Core.Tools;

public static class Calendar
{
    private static readonly TimeZoneInfo TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
    
    public static DateTime CurrentDateTime => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo);

    public static DateOnly CurrentDate => DateOnly.FromDateTime(CurrentDateTime);
}