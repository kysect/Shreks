// See https://aka.ms/new-console-template for more information

using Kysect.Shreks.DataImport;
using Newtonsoft.Json;
using Shreks.ApiClient;

Console.WriteLine();

const string baseUrl = "https://localhost:7188";
using var client = new HttpClient();

const string studentInfoFilePath = "student_info.json";
StudentInfo[] data = await GetStudentInfos(studentInfoFilePath);

IEnumerable<GroupName> groupNames = Enumerable.Range(0, 14)
    .Select(GroupName.FromShortName);

var identityClient = new IdentityClient(baseUrl, client);

LoginResponse loginResponse = await identityClient.LoginAsync(new LoginRequest
{
    Username = "ronimizy",
    Password = "1234567890aA!"
});

client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResponse.Token}");

var studyGroupClient = new StudyGroupClient(baseUrl, client);
var studentClient = new StudentClient(baseUrl, client);
var userClient = new UserClient(baseUrl, client) { ReadResponseAsString = true };

ICollection<StudyGroupDto> createdGroups = await studyGroupClient.StudyGroupGetAsync();
IEnumerable<GroupName> createdGroupNames = createdGroups.Select(x => new GroupName(x.Name));

groupNames = groupNames.Except(createdGroupNames);

var groups = new Dictionary<GroupName, StudyGroupDto>();

foreach (GroupName groupName in groupNames)
{
    StudyGroupDto group = await studyGroupClient.StudyGroupPostAsync(groupName.Name);
    groups[groupName] = group;
}

foreach (StudyGroupDto group in createdGroups)
{
    groups[new GroupName(group.Name)] = group;
}

foreach (StudentInfo studentInfo in data)
{
    try
    {
        UserDto user = await userClient.UserAsync(studentInfo.IsuNumber);
        /*
        await studentClient.GithubDeleteAsync(user.Id);
        await studentClient.GithubPostAsync(user.Id, studentInfo.GithubUsername);
        */
    }
    catch (ApiException e) when (e.StatusCode is 204)
    {
        (string firstName, string middleName, string lastName) = StudentName.FromString(studentInfo.FullName);
        StudyGroupDto group = groups[GroupName.FromShortName(int.Parse(studentInfo.Group))];
        StudentDto student = await studentClient.StudentPostAsync(firstName, middleName, lastName, group.Id);
        await userClient.UpdateAsync(student.User.Id, studentInfo.IsuNumber);
        await studentClient.GithubPostAsync(student.User.Id, studentInfo.GithubUsername);
    }
}

return;
/*
var subjectClient = new SubjectClient(baseUrl, client);
var subjectCourseController = new SubjectCourseClient(baseUrl, client);
var assignmentClient = new AssignmentsClient(baseUrl, client);
var subjectCourseGroupClient = new SubjectCourseGroupClient(baseUrl, client);
var groupAssignmentClient = new GroupAssignmentClient(baseUrl, client);

SubjectDto subject = await subjectClient.SubjectPostAsync("ООП");
SubjectCourseDto subjectCourse = await subjectCourseController.SubjectCoursePostAsync(subject.Id, "is-oop-y25");

var subjectCourseGroups = new List<SubjectCourseGroupDto>();
var assignments = new List<AssignmentDto>();

foreach (StudyGroupDto group in groups.Values)
{
    SubjectCourseGroupDto subjectCourseGroup =
        await subjectCourseGroupClient.SubjectCourseGroupPostAsync(subjectCourse.Id, group.Id);
    subjectCourseGroups.Add(subjectCourseGroup);
}

LabConfig[] labs =
{
    new("Isu", 0, 4, CreateDateTime(9, 10)), new("Shops", 1, 8, CreateDateTime(9, 24)),
    new("Isu.Extra", 2, 10, CreateDateTime(10, 8)), new("Backups", 3, 14, CreateDateTime(10, 22)),
    new("Banks", 4, 14, CreateDateTime(11, 5)), new("Backups.Extra", 5, 14, CreateDateTime(11, 19)),
    new("Messaging Service", 6, 16, CreateDateTime(12, 3))
};

IEnumerable<CreateAssignmentRequest> assignmentRequests = labs.Select(x => new CreateAssignmentRequest
{
    SubjectCourseId = subjectCourse.Id,
    Order = x.Index,
    ShortName = $"lab-{x.Index}",
    MinPoints = 0,
    MaxPoints = x.MaxPoints,
    Title = x.Title
});

foreach (CreateAssignmentRequest assignmentRequest in assignmentRequests)
{
    AssignmentDto assignment = await assignmentClient.AssignmentsPostAsync(assignmentRequest);
    assignments.Add(assignment);
}

IEnumerable<Task<GroupAssignmentDto>> groupAssignments = groups.Values
    .SelectMany(_ => assignments, (a, b) => (group: a, assignment: b))
    .Select(x =>
    {
        LabConfig lab = labs.Single(xx => xx.Index.Equals(x.assignment.Order));
        return groupAssignmentClient.GroupAssignmentPostAsync(x.group.Id, x.assignment.Id,
            new DateTimeOffset(lab.Deadline));
    });

foreach (Task<GroupAssignmentDto> task in groupAssignments)
    await task;


IEnumerable<Task> tasks = Enumerable.Range(1, 5).Select(x =>
{
    var span = TimeSpan.FromDays(x * 7);
    return subjectCourseController.FractionAsync(subjectCourse.Id, span, x * 0.2);
});

foreach (Task task in tasks)
    await task;

DateTime CreateDateTime(int month, int day)
{
    return new DateTime(2022, month, day);
}
*/

async Task<StudentInfo[]> GetStudentInfos(string filePath)
{
    string stringData = await File.ReadAllTextAsync(filePath);
    StudentInfo[]? studentInfos = JsonConvert.DeserializeObject<StudentInfo[]>(stringData);
    return studentInfos ?? throw new ArgumentException($"unable to parse {filePath}", nameof(filePath));
}

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
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Unable to parse empty StudentName", nameof(value));
            }

            string[] split = value.Trim()
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            string lastName = split.Length > 0 ? split[0] : string.Empty;
            string firstName = split.Length > 1 ? split[1] : string.Empty;
            string middleName = split.Length > 2 ? split[2] : string.Empty;

            return new StudentName(firstName, middleName, lastName);
        }
    }

    public readonly record struct GroupName(string Name)
    {
        public static GroupName FromShortName(int value)
        {
            return new GroupName($"M32{value:00}1");
        }
    }

    public readonly record struct LabConfig(string Title, int Index, double MaxPoints, DateTime Deadline);
}