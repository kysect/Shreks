using System.Text;
using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Application.Abstractions.Formatters;

public class UserFullNameFormatter : IUserFullNameFormatter
{
    public string GetFullName(UserDto user)
    {
        var fullNameBuilder = new StringBuilder()
            .Append(user.LastName)
            .Append(' ')
            .Append(user.FirstName);

        if (!string.IsNullOrEmpty(user.MiddleName))
        {
            fullNameBuilder
                .Append(' ')
                .Append(user.MiddleName);
        }

        return fullNameBuilder.ToString();
    }
}