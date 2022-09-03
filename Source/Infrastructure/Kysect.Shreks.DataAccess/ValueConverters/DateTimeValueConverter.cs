using Kysect.Shreks.Core.Tools;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kysect.Shreks.DataAccess.ValueConverters;

public class DateTimeValueConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeValueConverter() : base(
        x => x.ToUniversalTime(), 
        x => Calendar.Convert(DateTime.SpecifyKind(x, DateTimeKind.Utc))) {}
}