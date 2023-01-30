using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Mapping.Mappings;

public static class UserMapping
{
    public static UserDto ToDto(this User user)
        => new UserDto(user.Id, user.FirstName, user.MiddleName, user.LastName);
}