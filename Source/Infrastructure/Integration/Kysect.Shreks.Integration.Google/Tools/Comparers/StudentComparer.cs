using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Integration.Google.Tools.Comparers;

public static class StudentComparer
{
    public static bool ShouldBeSeparated(StudentDto? student1, StudentDto? student2)
    {
        if (student1 is null || student2 is null)
            return true;

        return student1.GroupName != student2.GroupName;
    }
}