using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.GoogleIntegration.Providers;

public class StudentSheetIdentifierProvider : IStudentIdentifierProvider
{
    private readonly IUserFullNameFormatter _userFullNameFormatter;

    public StudentSheetIdentifierProvider(IUserFullNameFormatter userFullNameFormatter)
    {
        _userFullNameFormatter = userFullNameFormatter;
    }
    
    public string GetStudentIdentifier(Student student)
        => $"{_userFullNameFormatter.GetFullName(student)}{student.Group.Name}";
}