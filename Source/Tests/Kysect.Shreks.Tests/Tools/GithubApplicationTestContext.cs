using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Tests.Tools;

public record GithubApplicationTestContext(
    GithubSubjectCourseAssociation SubjectCourseAssociation,
    Student Student);