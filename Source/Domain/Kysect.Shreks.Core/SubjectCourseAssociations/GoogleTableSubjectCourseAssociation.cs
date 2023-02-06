using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.SubjectCourseAssociations;

public partial class GoogleTableSubjectCourseAssociation : SubjectCourseAssociation
{
    public GoogleTableSubjectCourseAssociation(Guid id, SubjectCourse subjectCourse, string spreadsheetId)
        : base(id, subjectCourse)
    {
        SpreadsheetId = spreadsheetId;
    }

    public string SpreadsheetId { get; protected set; }
}