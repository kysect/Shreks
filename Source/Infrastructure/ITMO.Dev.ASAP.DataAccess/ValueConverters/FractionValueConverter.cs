using ITMO.Dev.ASAP.Core.ValueObject;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ITMO.Dev.ASAP.DataAccess.ValueConverters;

public class FractionValueConverter : ValueConverter<Fraction, double>
{
    public FractionValueConverter()
        : base(x => x.Value, x => new Fraction(x)) { }
}