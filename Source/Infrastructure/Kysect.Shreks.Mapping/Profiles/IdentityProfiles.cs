using AutoMapper;
using Kysect.Shreks.Application.Dto.Identity;
using Kysect.Shreks.Identity.Entities;

namespace Kysect.Shreks.Mapping.Profiles;

public class IdentityProfiles : Profile
{
    public IdentityProfiles()
    {
        CreateMap<ShreksIdentityUser, IdentityUserDto>()
            .ForCtorParam(nameof(IdentityUserDto.Username), opt => opt.MapFrom(x => x.UserName));
    }
}