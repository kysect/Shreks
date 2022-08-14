using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubjectCourseGroupGenerator : EntityGeneratorBase<SubjectCourseGroup>
{
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly Faker _faker;
    
    public SubjectCourseGroupGenerator(
        EntityGeneratorOptions<SubjectCourseGroup> options,
        IEntityGenerator<SubjectCourse> subjectCourseGenerator,
        IEntityGenerator<StudentGroup> studentGroupGenerator,
        Faker faker) 
        : base(options)
    {
        _subjectCourseGenerator = subjectCourseGenerator;
        _studentGroupGenerator = studentGroupGenerator;
        _faker = faker;
    }

    protected override SubjectCourseGroup Generate(int index)
    {
        var studentGroupCount = _studentGroupGenerator.GeneratedEntities.Count;
        var studentGroupNumber = index % studentGroupCount;
        
        var subjectCourseCount = _subjectCourseGenerator.GeneratedEntities.Count;
        var subjectCourseGroupNumber = index / studentGroupCount;

        if (subjectCourseGroupNumber >= subjectCourseCount)
            throw new IndexOutOfRangeException(
                "The subject course group index is greater than the number of subject courses.");

        var subjectCourse = _subjectCourseGenerator.GeneratedEntities[subjectCourseGroupNumber];
        var studentGroup = _studentGroupGenerator.GeneratedEntities[studentGroupNumber];
        
        var subjectCourseGroup = new SubjectCourseGroup(subjectCourse, studentGroup);
        
        subjectCourse.AddGroup(subjectCourseGroup);

        return subjectCourseGroup;
    }
}