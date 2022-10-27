// See https://aka.ms/new-console-template for more information

using Kysect.Shreks.DataImport;
using Newtonsoft.Json;
using Shreks.ApiClient;

Console.WriteLine();

const string baseUrl = "https://localhost:7188";
using var client = new HttpClient();

var stringData = await File.ReadAllTextAsync("student_info.json");
var data = JsonConvert.DeserializeObject<StudentInfo[]>(stringData);

_ = data ?? throw new ArgumentNullException(nameof(stringData));

var groupNames = Enumerable.Range(0, 14)
    .Select(GroupName.FromShortName);

var identityClient = new IdentityClient(baseUrl, client);

var loginResponse = await identityClient.LoginAsync(new LoginRequest
{
    Username = "ronimizy",
    Password = "1234567890aA!",
});

client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResponse.Token}");

var studyGroupClient = new StudyGroupClient(baseUrl, client);
var studentClient = new StudentClient(baseUrl, client);
var userClient = new UserClient(baseUrl, client)
{
    ReadResponseAsString = true,
};

var createdGroups = await studyGroupClient.StudyGroupGetAsync();
var createdGroupNames = createdGroups.Select(x => new GroupName(x.Name));

groupNames = groupNames.Except(createdGroupNames);

var groups = new Dictionary<GroupName, StudyGroupDto>();

foreach (var groupName in groupNames)
{
    var group = await studyGroupClient.StudyGroupPostAsync(groupName.Name);
    groups[groupName] = group;
}

foreach (var group in createdGroups)
{
    groups[new GroupName(group.Name)] = group;
}

foreach (var studentInfo in data)
{
    try
    {
        var user = await userClient.UserAsync(studentInfo.IsuNumber);
        continue;
        await studentClient.GithubDeleteAsync(user.Id);
        await studentClient.GithubPostAsync(user.Id, studentInfo.GithubUsername);
    }
    catch (ApiException e) when (e.StatusCode is 204)
    {
        var (firstName, middleName, lastName) = StudentName.FromString(studentInfo.FullName);
        var group = groups[GroupName.FromShortName(int.Parse(studentInfo.Group))];
        var student = await studentClient.StudentPostAsync(firstName, middleName, lastName, group.Id);
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

var subject = await subjectClient.SubjectPostAsync("ООП");
var subjectCourse = await subjectCourseController.SubjectCoursePostAsync(subject.Id, "is-oop-y25");

var subjectCourseGroups = new List<SubjectCourseGroupDto>();
var assignments = new List<AssignmentDto>();

foreach (var group in groups.Values)
{
    var subjectCourseGroup = await subjectCourseGroupClient.SubjectCourseGroupPostAsync(subjectCourse.Id, group.Id);
    subjectCourseGroups.Add(subjectCourseGroup);
}

var labs = new[]
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

foreach (var assignmentRequest in assignmentRequests)
{
    var assignment = await assignmentClient.AssignmentsPostAsync(assignmentRequest);
    assignments.Add(assignment);
}

var groupAssignments = groups.Values
    .SelectMany(_ => assignments, (a, b) => (group: a, assignment: b))
    .Select(x =>
    {
        var lab = labs.Single(xx => xx.Index.Equals(x.assignment.Order));
        return groupAssignmentClient.GroupAssignmentPostAsync(x.group.Id, x.assignment.Id,
            new DateTimeOffset(lab.Deadline));
    });

foreach (Task<GroupAssignmentDto> task in groupAssignments)
{
    await task;
}


var tasks = Enumerable.Range(1, 5).Select(x =>
{
    TimeSpan span = TimeSpan.FromDays(x * 7);
    return subjectCourseController.FractionAsync(subjectCourse.Id, span, x * 0.2);
});

foreach (var task in tasks)
{
    await task;
}

DateTime CreateDateTime(int month, int day)
    => new DateTime(2022, month, day);

namespace Kysect.Shreks.DataImport
{
    public readonly record struct StudentInfo(
        string FullName,
        string Group,
        string GithubUsername,
        string TelegramTag,
        int IsuNumber,
        DateTime Submitted);

    public readonly record struct StudentName(string FirstName, string MiddleName, string LastName)
    {
        public static StudentName FromString(string value)
        {
            var split = value.Trim().Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var lastName = split.Length > 0 ? split[0] : string.Empty;
            var firstName = split.Length > 1 ? split[1] : string.Empty;
            var middleName = split.Length > 2 ? split[2] : string.Empty;

            return new StudentName(firstName, middleName, lastName);
        }
    }

    public readonly record struct GroupName(string Name)
    {
        public static GroupName FromShortName(int value)
            => new GroupName($"M32{value:00}1");
    }

    public readonly record struct LabConfig(string Title, int Index, double MaxPoints, DateTime Deadline);
}