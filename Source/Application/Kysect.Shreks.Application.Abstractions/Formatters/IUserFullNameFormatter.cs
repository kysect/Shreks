using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Application.Abstractions.Formatters;

public interface IUserFullNameFormatter
{
    string GetFullName(UserDto user);
}