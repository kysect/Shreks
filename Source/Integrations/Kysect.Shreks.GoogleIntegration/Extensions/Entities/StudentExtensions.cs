using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.GoogleIntegration.Extensions.Entities;

public static class StudentExtensions
{
    public static string GetFullName(this Student student)
        => $"{student.LastName} {student.FirstName} {student.MiddleName}";
}