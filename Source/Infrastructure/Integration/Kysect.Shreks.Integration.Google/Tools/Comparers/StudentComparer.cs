using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Integration.Google.Tools.Comparers;

public static class StudentComparer
{
    public static bool InDifferentGroups(StudentDto? student1, StudentDto? student2)
    {
        return student1?.GroupName != student2?.GroupName;
    }
}