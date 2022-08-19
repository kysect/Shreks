using Kysect.Shreks.Core.ValueObject;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kysect.Shreks.DataAccess.ValueConverters;

public class RatingValueConverter : ValueConverter<Rating, double>
{
    public RatingValueConverter() : base(x => x.Value, x => new Rating(x)) { }
}