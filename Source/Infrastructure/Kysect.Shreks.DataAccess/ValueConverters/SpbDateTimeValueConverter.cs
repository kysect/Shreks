using Kysect.Shreks.Core.Tools;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kysect.Shreks.DataAccess.ValueConverters;

public class SpbDateTimeValueConverter : ValueConverter<SpbDateTime, DateTime>
{
    public SpbDateTimeValueConverter() : base(
        x => Calendar.ToUtc(x),
        x => Calendar.FromUtc(x)) { }
}