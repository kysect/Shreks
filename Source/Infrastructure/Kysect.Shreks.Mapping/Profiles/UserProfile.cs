using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Mapping.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Student, StudentDto>()
            .ForCtorParam(nameof(StudentDto.UniversityId),
                opt => opt.MapFrom(x => x.User.FindAssociation<IsuUserAssociation>()!.UniversityId))
            .ForCtorParam(nameof(StudentDto.GitHubUsername),
                opt => opt.MapFrom(x => x.User.FindAssociation<GithubUserAssociation>()!.GithubUsername));
    }
}