using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ITMO.Dev.ASAP.DataAccess.ValueConverters;

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(
        x => x.ToDateTime(new TimeOnly(0)),
        x => DateOnly.FromDateTime(x))
    {
    }
}