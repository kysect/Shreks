﻿using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class StudentSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Student> _generator;

    public int Priority => 1;

    public StudentSeeder(IEntityGenerator<Student> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.Students.AddRange(_generator.GeneratedEntities);
}