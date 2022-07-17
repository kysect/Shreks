using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.SubjectCourseAssociations;

public abstract partial class SubjectCourseAssociation : IEntity<Guid>
{
    protected SubjectCourseAssociation(SubjectCourse subjectCourse) : this(Guid.NewGuid())
    {
        SubjectCourse = subjectCourse;
    }

    public virtual SubjectCourse SubjectCourse { get; protected init; }
}