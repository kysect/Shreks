using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.Formatters;

public interface IUserFullNameFormatter
{
    string GetFullName(User user);
}