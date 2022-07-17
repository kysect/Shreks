using Kysect.Shreks.Core.ValueObject;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kysect.Shreks.DataAccess.ValueConverters;

public class FractionValueConverter : ValueConverter<Fraction, double>
{
    public FractionValueConverter() : base(x => x.Value, x => new Fraction(x)) { }
}