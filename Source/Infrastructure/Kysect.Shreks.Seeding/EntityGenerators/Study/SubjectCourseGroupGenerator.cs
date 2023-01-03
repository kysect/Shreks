using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubjectCourseGroupGenerator : EntityGeneratorBase<SubjectCourseGroup>
{
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;

    public SubjectCourseGroupGenerator(
        EntityGeneratorOptions<SubjectCourseGroup> options,
        IEntityGenerator<SubjectCourse> subjectCourseGenerator,
        IEntityGenerator<StudentGroup> studentGroupGenerator)
        : base(options)
    {
        _subjectCourseGenerator = subjectCourseGenerator;
        _studentGroupGenerator = studentGroupGenerator;
    }

    protected override SubjectCourseGroup Generate(int index)
    {
        int studentGroupCount = _studentGroupGenerator.GeneratedEntities.Count;
        int studentGroupNumber = index % studentGroupCount;

        int subjectCourseCount = _subjectCourseGenerator.GeneratedEntities.Count;
        int subjectCourseGroupNumber = index / studentGroupCount;

        if (subjectCourseGroupNumber >= subjectCourseCount)
        {
            const string message = "The subject course group index is greater than the number of subject courses.";
            throw new ArgumentOutOfRangeException(nameof(index), message);
        }

        SubjectCourse subjectCourse = _subjectCourseGenerator.GeneratedEntities[subjectCourseGroupNumber];
        StudentGroup studentGroup = _studentGroupGenerator.GeneratedEntities[studentGroupNumber];

        return subjectCourse.AddGroup(studentGroup);
    }
}