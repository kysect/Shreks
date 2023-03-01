// See https://aka.ms/new-console-template for more information

#pragma warning disable CA1506

using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.DataImport;
using ITMO.Dev.ASAP.DataImport.Models;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Students;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.StudyGroups;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Identity;
using ITMO.Dev.ASAP.WebApi.Sdk.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

string stringData = await File.ReadAllTextAsync("student_info.json");
StudentInfo[]? data = JsonConvert.DeserializeObject<StudentInfo[]>(stringData);

_ = data ?? throw new Exception("Deserialization failed");

Console.WriteLine();

string baseUrl = ReadLabeled("Uri: ");

var identityProvider = new IdentityProvider();

var collection = new ServiceCollection();
collection.AddAsapSdk(new Uri(baseUrl));
collection.AddSingleton<IIdentityProvider>(identityProvider);

ServiceProvider provider = collection.BuildServiceProvider();

IEnumerable<GroupName> groupNames = data.Select(x => new GroupName(x.Group)).Distinct();

IIdentityClient identityClient = provider.GetRequiredService<IIdentityClient>();
IStudyGroupClient studyGroupClient = provider.GetRequiredService<IStudyGroupClient>();
IStudentClient studentClient = provider.GetRequiredService<IStudentClient>();
IUserClient userClient = provider.GetRequiredService<IUserClient>();

string username = ReadLabeled("Username: ");
string password = ReadLabeled("Password: ");

var loginRequest = new LoginRequest(username, password);
LoginResponse loginResponse = await identityClient.LoginAsync(loginRequest);
identityProvider.UserIdentity = new UserIdentity(loginResponse.Token, loginResponse.Expires);

IReadOnlyCollection<StudyGroupDto> createdGroups = await studyGroupClient.QueryAsync(
    new QueryConfiguration<GroupQueryParameter>(ArraySegment<QueryParameter<GroupQueryParameter>>.Empty));

IEnumerable<GroupName> createdGroupNames = createdGroups.Select(x => new GroupName(x.Name));
groupNames = groupNames.Except(createdGroupNames);

var groups = new Dictionary<GroupName, StudyGroupDto>();

foreach (GroupName groupName in groupNames)
{
    StudyGroupDto group = await studyGroupClient.CreateAsync(new CreateStudyGroupRequest(groupName.Name));
    groups[groupName] = group;
}

foreach (StudyGroupDto? group in createdGroups)
{
    groups[new GroupName(group.Name)] = group;
}

foreach (StudentInfo studentInfo in data)
{
    UserDto? user = await userClient.FindUserByUniversityIdAsync(studentInfo.IsuNumber);

    if (user is null)
    {
        Console.WriteLine($"Creating: {studentInfo}");
        (string? firstName, string? middleName, string? lastName) = StudentName.FromString(studentInfo.FullName);
        StudyGroupDto group = groups[new GroupName(studentInfo.Group)];

        var createStudentRequest = new CreateStudentRequest(firstName, middleName, lastName, group.Id);
        StudentDto student = await studentClient.CreateAsync(createStudentRequest);

        await userClient.UpdateUniversityIdAsync(student.User.Id, studentInfo.IsuNumber);
        await studentClient.AddGithubAssociationAsync(student.User.Id, studentInfo.GithubUsername);
    }
    else
    {
        // await studentClient.RemoveGithubAssociationAsync(user.Id);
        // await studentClient.AddGithubAssociationAsync(user.Id, studentInfo.GithubUsername);
    }
}

return;

static string ReadLabeled(string label)
{
    Console.Write(label);
    return Console.ReadLine() ?? string.Empty;
}