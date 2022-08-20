using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.SubjectCourseAssociations;

public partial class GoogleTableSubjectCourseAssociation : SubjectCourseAssociation
{
    public GoogleTableSubjectCourseAssociation(SubjectCourse subjectCourse, string spreadsheetId)
        : base(subjectCourse)
    {
        SpreadsheetId = spreadsheetId;
    }

    public string SpreadsheetId { get; protected set; }
}