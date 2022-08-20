﻿using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubmissionGenerator : EntityGeneratorBase<Submission>
{
    private const double MaxExtraPoints = 15;
    private const float ChangeOfHavingExtraPoints = 0.1f;

    private readonly Faker _faker;
    private readonly IEntityGenerator<Student> _studentGenerator;
    private readonly IEntityGenerator<Assignment> _assignmentGenerator;

    public SubmissionGenerator(
        EntityGeneratorOptions<Submission> options,
        IEntityGenerator<Student> studentGenerator,
        IEntityGenerator<Assignment> assignmentGenerator,
        Faker faker) : base(options)
    {
        _faker = faker;
        _assignmentGenerator = assignmentGenerator;
        _studentGenerator = studentGenerator;
    }

    protected override Submission Generate(int index)
    {
        var assignment = _faker.PickRandom<Assignment>(_assignmentGenerator.GeneratedEntities);
        var student = _faker.PickRandom<Student>(_studentGenerator.GeneratedEntities);

        var submission = new Submission
        (
            student,
            assignment,
            DateOnly.FromDateTime(_faker.Date.Future())
        )
        {
            Rating = _faker.Random.Fraction(),
            ExtraPoints = _faker.Random.Bool(ChangeOfHavingExtraPoints) ? _faker.Random.Points(0, MaxExtraPoints) : Points.None,
        };

        submission.UpdateAssociation(new GithubPullRequestSubmissionAssociation(submission, _faker.Random.Int(1, 100)));
        return submission;
    }
}