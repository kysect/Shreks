using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.Application.Abstractions.Formatters;

public interface IUserFullNameFormatter
{
    string GetFullName(UserDto user);
}