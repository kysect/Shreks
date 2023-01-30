using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Mapping.Mappings;

public static class StudentMapping
{
    public static StudentDto ToDto(this Student student)
    {
        IsuUserAssociation? isuAssociation = student.User.FindAssociation<IsuUserAssociation>();
        GithubUserAssociation? githubAssociation = student.User.FindAssociation<GithubUserAssociation>();

        return new StudentDto(
            student.User.ToDto(),
            student.Group?.Name ?? string.Empty,
            isuAssociation?.UniversityId,
            githubAssociation?.GithubUsername);
    }
}