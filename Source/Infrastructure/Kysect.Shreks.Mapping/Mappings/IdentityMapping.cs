using Kysect.Shreks.Application.Dto.Identity;
using Kysect.Shreks.Identity.Entities;

namespace Kysect.Shreks.Mapping.Mappings;

public static class IdentityMapping
{
    public static IdentityUserDto ToDto(this ShreksIdentityUser user)
        => new IdentityUserDto(user.UserName);
}