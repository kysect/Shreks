using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.SubjectCourseAssociations;

public abstract partial class SubjectCourseAssociation : IEntity<Guid>
{
    protected SubjectCourseAssociation(Guid id, SubjectCourse subjectCourse) : this(id)
    {
        SubjectCourse = subjectCourse;
    }

    public virtual SubjectCourse SubjectCourse { get; protected init; }
}