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

        CreateMap<Fraction, double>()
            .ConstructUsing(x => x.Value)
            .ReverseMap()
            .ConstructUsing(x => new Fraction(x));
    }
}