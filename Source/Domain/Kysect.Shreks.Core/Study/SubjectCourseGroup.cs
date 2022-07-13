using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class SubjectCourseGroup : IEntity
{
    private readonly HashSet<Mentor> _practiceMentors;

    public SubjectCourseGroup(SubjectCourse subjectCourse, StudentGroup studentGroup)
        : this(subjectCourseId: subjectCourse.Id, studentGroupId: studentGroup.Id)
    {
        ArgumentNullException.ThrowIfNull(subjectCourse);
        ArgumentNullException.ThrowIfNull(studentGroup);

        SubjectCourse = subjectCourse;
        StudentGroup = studentGroup;
        _practiceMentors = new HashSet<Mentor>();
    }

    [KeyProperty]
    public virtual SubjectCourse SubjectCourse { get; protected init; }

    [KeyProperty]
    public virtual StudentGroup StudentGroup { get; protected init; }

    public virtual IReadOnlyCollection<Mentor> PracticeMentors => _practiceMentors;

    public override string ToString()
        => StudentGroup.ToString();

    public void AddPracticeMentor(Mentor mentor)
    {
        ArgumentNullException.ThrowIfNull(mentor);

        if (!_practiceMentors.Add(mentor))
            throw new DomainInvalidOperationException($"Mentor {mentor.Id} is already assigned to this group");
    }

    public void RemovePracticeMentor(Mentor mentor)
    {
        ArgumentNullException.ThrowIfNull(mentor);

        if (!_practiceMentors.Remove(mentor))
            throw new DomainInvalidOperationException($"Mentor {mentor.Id} could not be removed from this group");
    }
}