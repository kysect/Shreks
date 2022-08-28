using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kysect.Shreks.DataAccess.ValueConverters;

public class DateOnlyConverter : ValueConverter<DateOnly, long>
{
    public DateOnlyConverter() : base(
        x => x.ToDateTime(new TimeOnly(0)).Ticks,
        x => DateOnly.FromDateTime(new DateTime(x))) { }
}