using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.WebUI.AdminPanel.Extensions;

public static class StudentDtoExtensions
{
    public static string FullName(this StudentDto student)
        => $"{student.User.LastName} {student.User.FirstName} {student.User.MiddleName}";

    public static string DisplayString(this StudentDto student)
        => $"{student.UniversityId} {student.User.LastName} {student.User.FirstName} {student.User.MiddleName}";
}