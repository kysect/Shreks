﻿using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubjectCourseGenerator : EntityGeneratorBase<SubjectCourse>
{
    private readonly Faker _faker;
    private readonly IEntityGenerator<Mentor> _mentorGenerator;
    private readonly IEntityGenerator<Subject> _subjectGenerator;

    public SubjectCourseGenerator(
        EntityGeneratorOptions<SubjectCourse> options,
        IEntityGenerator<Mentor> mentorGenerator,
        IEntityGenerator<Subject> subjectGenerator,
        Faker faker) 
        : base(options)
    {
        _mentorGenerator = mentorGenerator;
        _subjectGenerator = subjectGenerator;
        _faker = faker;
    }

    protected override SubjectCourse Generate(int index)
    {
        var mentorCount = _mentorGenerator.GeneratedEntities.Count;
        var mentorNumber = _faker.Random.Number(0, mentorCount - 1);

        var mentor = _mentorGenerator.GeneratedEntities[mentorNumber];
        
        var subjectCount = _subjectGenerator.GeneratedEntities.Count;
        var subjectNumber = _faker.Random.Number(0, subjectCount - 1);

        var subject = _subjectGenerator.GeneratedEntities[subjectNumber];

        var subjectCourse = new SubjectCourse(subject, mentor);
        
        subject.AddCourse(subjectCourse);

        return subjectCourse;
    }
}