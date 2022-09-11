using Kysect.Shreks.Core.Queue;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class SubjectCourseGroup : IEntity
{
    public SubjectCourseGroup(SubjectCourse subjectCourse, StudentGroup studentGroup, SubmissionQueue queue)
        : this(subjectCourseId: subjectCourse.Id, studentGroupId: studentGroup.Id)
    {
        ArgumentNullException.ThrowIfNull(subjectCourse);
        ArgumentNullException.ThrowIfNull(studentGroup);

        SubjectCourse = subjectCourse;
        StudentGroup = studentGroup;
        Queue = queue;
    }

    [KeyProperty]
    public virtual SubjectCourse SubjectCourse { get; protected init; }

    [KeyProperty]
    public virtual StudentGroup StudentGroup { get; protected init; }
    
    public virtual SubmissionQueue Queue { get; set; }

    public override string ToString()
        => StudentGroup.ToString();
}