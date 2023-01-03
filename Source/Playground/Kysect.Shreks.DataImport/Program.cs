// See https://aka.ms/new-console-template for more information

using Kysect.Shreks.DataImport;
using Newtonsoft.Json;
using Shreks.ApiClient;

Console.WriteLine();
#pragma warning disable CS0162

const string baseUrl = "https://localhost:7188";
using var client = new HttpClient();

string? stringData = await File.ReadAllTextAsync("student_info.json");
StudentInfo[]? data = JsonConvert.DeserializeObject<StudentInfo[]>(stringData);

_ = data ?? throw new Exception("Deserialization failed");

IEnumerable<GroupName>? groupNames = Enumerable.Range(0, 14)
    .Select(GroupName.FromShortName);

var identityClient = new IdentityClient(baseUrl, client);

LoginResponse? loginResponse = await identityClient.LoginAsync(new LoginRequest
{
    Username = "",
    Password = "",
});

client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResponse.Token}");

var studyGroupClient = new StudyGroupClient(baseUrl, client);
var studentClient = new StudentClient(baseUrl, client);
var userClient = new UserClient(baseUrl, client)
{
    ReadResponseAsString = true,
};

ICollection<StudyGroupDto>? createdGroups = await studyGroupClient.StudyGroupGetAsync();
IEnumerable<GroupName>? createdGroupNames = createdGroups.Select(x => new GroupName(x.Name));

groupNames = groupNames.Except(createdGroupNames);

var groups = new Dictionary<GroupName, StudyGroupDto>();

foreach (GroupName groupName in groupNames)
{
    StudyGroupDto group = await studyGroupClient.StudyGroupPostAsync(groupName.Name);
    groups[groupName] = group;
}

foreach (StudyGroupDto? group in createdGroups)
{
    groups[new GroupName(group.Name)] = group;
}

foreach (StudentInfo studentInfo in data)
{
    try
    {
        UserDto? user = await userClient.UserAsync(studentInfo.IsuNumber);
        continue;
        await studentClient.GithubDeleteAsync(user.Id);
        await studentClient.GithubPostAsync(user.Id, studentInfo.GithubUsername);
    }
    catch (ApiException e) when (e.StatusCode is 204)
    {
        (string? firstName, string? middleName, string? lastName) = StudentName.FromString(studentInfo.FullName);
        StudyGroupDto? group = groups[GroupName.FromShortName(int.Parse(studentInfo.Group))];
        StudentDto? student = await studentClient.StudentPostAsync(firstName, middleName, lastName, group.Id);
        await userClient.UpdateAsync(student.User.Id, studentInfo.IsuNumber);
        await studentClient.GithubPostAsync(student.User.Id, studentInfo.GithubUsername);
    }
}

return;

var subjectClient = new SubjectClient(baseUrl, client);
var subjectCourseController = new SubjectCourseClient(baseUrl, client);
var assignmentClient = new AssignmentsClient(baseUrl, client);
var subjectCourseGroupClient = new SubjectCourseGroupClient(baseUrl, client);
var groupAssignmentClient = new GroupAssignmentClient(baseUrl, client);

SubjectDto? subject = await subjectClient.SubjectPostAsync("ООП");
SubjectCourseDto? subjectCourse = await subjectCourseController.SubjectCoursePostAsync(subject.Id, "is-oop-y25");

var subjectCourseGroups = new List<SubjectCourseGroupDto>();
var assignments = new List<AssignmentDto>();

foreach (StudyGroupDto? group in groups.Values)
{
    SubjectCourseGroupDto? subjectCourseGroup =
        await subjectCourseGroupClient.SubjectCourseGroupPostAsync(subjectCourse.Id, group.Id);
    subjectCourseGroups.Add(subjectCourseGroup);
}

LabConfig[]? labs = new[]
{
    new LabConfig("Isu", 0, 4, CreateDateTime(9, 10)),
    new LabConfig("Shops", 1, 8, CreateDateTime(9, 24)),
    new LabConfig("Isu.Extra", 2, 10, CreateDateTime(10, 8)),
    new LabConfig("Backups", 3, 14, CreateDateTime(10, 22)),
    new LabConfig("Banks", 4, 14, CreateDateTime(11, 5)),
    new LabConfig("Backups.Extra", 5, 14, CreateDateTime(11, 19)),
    new LabConfig("Messaging Service", 6, 16, CreateDateTime(12, 3)),
};

IEnumerable<CreateAssignmentRequest> assignmentRequests = labs.Select(x => new CreateAssignmentRequest
{
    SubjectCourseId = subjectCourse.Id,
    Order = x.Index,
    ShortName = $"lab-{x.Index}",
    MinPoints = 0,
    MaxPoints = x.MaxPoints,
    Title = x.Title,
});

foreach (CreateAssignmentRequest? assignmentRequest in assignmentRequests)
{
    AssignmentDto? assignment = await assignmentClient.AssignmentsPostAsync(assignmentRequest);
    assignments.Add(assignment);
}

IEnumerable<Task<GroupAssignmentDto>>? groupAssignments = groups.Values
    .SelectMany(_ => assignments, (a, b) => (group: a, assignment: b))
    .Select(x =>
    {
        LabConfig lab = labs.Single(xx => xx.Index.Equals(x.assignment.Order));
        return groupAssignmentClient.GroupAssignmentPostAsync(x.group.Id, x.assignment.Id,
            new DateTimeOffset(lab.Deadline));
    });

foreach (Task<GroupAssignmentDto> task in groupAssignments)
    await task;

IEnumerable<Task>? tasks = Enumerable.Range(1, 5).Select(x =>
{
    var span = TimeSpan.FromDays(x * 7);
    return subjectCourseController.FractionAsync(subjectCourse.Id, span, x * 0.2);
});

foreach (Task? task in tasks)
{
    await task;
}

DateTime CreateDateTime(int month, int day)
{
    return new DateTime(2022, month, day);
}