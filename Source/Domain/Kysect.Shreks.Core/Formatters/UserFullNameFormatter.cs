using System.Text;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.Formatters;

public record UserFullNameFormatter(User User) : IFullNameFormatter
{
    public string GetFullName()
    {
        var fullNameBuilder = new StringBuilder()
            .Append(User.LastName)
            .Append(' ')
            .Append(User.FirstName);

        if (User.MiddleName != "")
        {
            fullNameBuilder
                .Append(' ')
                .Append(User.MiddleName);
        }

        return fullNameBuilder.ToString();
    }
}