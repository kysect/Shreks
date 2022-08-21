using AutoMapper;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Mapping.Profiles;

public class ValueObjectsProfile : Profile
{
    public ValueObjectsProfile()
    {
        CreateMap<Points, double>()
            .ConstructUsing(x => x.Value)
            .ReverseMap()
            .ConstructUsing(x => new Points(x));

        CreateMap<Points?, double?>()
            .ConstructUsing(x => x == null ? null : x.Value.Value)
            .ReverseMap()
            .ConstructUsing(x => x == null ? null : new Points(x.Value));

        CreateMap<Fraction, double>()
            .ConstructUsing(x => x.Value)
            .ReverseMap()
            .ConstructUsing(x => new Fraction(x));

        CreateMap<Fraction?, double?>()
            .ConstructUsing(x => x == null ? null : x.Value.Value)
            .ReverseMap()
            .ConstructUsing(x => x == null ? null : new Fraction(x.Value));
    }
}