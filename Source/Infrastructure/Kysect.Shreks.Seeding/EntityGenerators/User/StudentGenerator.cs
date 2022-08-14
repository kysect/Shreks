﻿using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class StudentGenerator : EntityGeneratorBase<Student>
{
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly IEntityGenerator<User> _userGenerator;
    private readonly Faker _faker;

    public StudentGenerator(
        EntityGeneratorOptions<Student> options,
        IEntityGenerator<StudentGroup> studentGroupGenerator,
        Faker faker,
        IEntityGenerator<User> userGenerator) : base(options)
    {
        _studentGroupGenerator = studentGroupGenerator;
        _faker = faker;
        _userGenerator = userGenerator;
    }

    protected override Student Generate(int index)
    {
        var groupCount = _studentGroupGenerator.GeneratedEntities.Count;
        var groupNumber = _faker.Random.Number(0, groupCount - 1);

        StudentGroup group = _studentGroupGenerator.GeneratedEntities[groupNumber];

        var userIndex = _faker.Random.Int(0, _userGenerator.GeneratedEntities.Count - 1);
        var user = _userGenerator.GeneratedEntities[userIndex];

        var student = new Student(user, group);

        group.AddStudent(student);

        return student;
    }
}