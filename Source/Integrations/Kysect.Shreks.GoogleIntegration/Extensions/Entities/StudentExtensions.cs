using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.GoogleIntegration.Extensions.Entities;

internal static class StudentExtensions
{
    public static string GetFullName(this Student student)
        => new UserFullNameFormatter(student).GetFullName();

    public static string GetSheetIdentifier(this Student student)
        => $"{student.GetFullName()}{student.Group.Name}";
}