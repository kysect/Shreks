using Ardalis.Result;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class SubjectCourseGroup : IEntity
{
    private readonly List<Mentor> _practiceMentors;

    public SubjectCourseGroup(SubjectCourse subjectCourse, StudentGroup studentGroup)
        : this(subjectCourseId: subjectCourse.Id, studentGroupId: studentGroup.Id)
    {
        ArgumentNullException.ThrowIfNull(subjectCourse);
        ArgumentNullException.ThrowIfNull(studentGroup);

        SubjectCourse = subjectCourse;
        StudentGroup = studentGroup;
        _practiceMentors = new List<Mentor>();
    }

    [KeyProperty]
    public virtual SubjectCourse SubjectCourse { get; protected init; }

    [KeyProperty]
    public virtual StudentGroup StudentGroup { get; protected init; }

    public virtual IReadOnlyCollection<Mentor> PracticeMentors => _practiceMentors.AsReadOnly();

    public override string ToString()
        => StudentGroup.ToString();

    public Result AddPracticeMentor(Mentor mentor)
    {
        ArgumentNullException.ThrowIfNull(mentor);
        
        if (_practiceMentors.Contains(mentor))
            return Result.Error($"Mentor {mentor.Id} is already assigned to this group");
        
        _practiceMentors.Add(mentor);
        return Result.Success();
    }
    
    public Result RemovePracticeMentor(Mentor mentor)
    {
        ArgumentNullException.ThrowIfNull(mentor);
        
        if (!_practiceMentors.Contains(mentor))
            return Result.Error($"Mentor {mentor.Id} is not assigned to this group");
        
        if (!_practiceMentors.Remove(mentor))
            return Result.Error($"Mentor {mentor.Id} could not be removed from this group");
        
        return Result.Success();
    }
}