using System.Text;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.Formatters;

public class UserFullNameFormatter : IUserFullNameFormatter
{
    public string GetFullName(User user)
    {
        var fullNameBuilder = new StringBuilder()
            .Append(user.LastName)
            .Append(' ')
            .Append(user.FirstName);

        if (user.MiddleName != "")
        {
            fullNameBuilder
                .Append(' ')
                .Append(user.MiddleName);
        }

        return fullNameBuilder.ToString();
    }
}