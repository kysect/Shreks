using Kysect.Shreks.Core.ValueObject;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kysect.Shreks.DataAccess.ValueConverters;

public class PointsValueConverter : ValueConverter<Points, double>
{
    public PointsValueConverter()
        : base(x => x.Value, x => new Points(x)) { }
}