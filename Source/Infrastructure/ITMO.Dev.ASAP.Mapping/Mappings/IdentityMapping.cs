using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Identity.Entities;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class IdentityMapping
{
    public static IdentityUserDto ToDto(this AsapIdentityUser user)
    {
        return new IdentityUserDto(user.UserName);
    }
}