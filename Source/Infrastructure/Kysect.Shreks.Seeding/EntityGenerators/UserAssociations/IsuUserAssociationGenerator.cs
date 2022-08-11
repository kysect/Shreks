using Bogus;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class IsuUserAssociationGenerator : EntityGeneratorBase<IsuUserAssociation>
{
    private const int MinIsuNumber = 100000;
    private const int MaxIsuNumber = 1000000;

    private readonly IEntityGenerator<Mentor> _mentorGenerator;
    private readonly IEntityGenerator<Student> _studentGenerator;

    private readonly Faker _faker;

    public IsuUserAssociationGenerator(
        EntityGeneratorOptions<IsuUserAssociation> options,
        IEntityGenerator<Mentor> mentorGenerator,
        IEntityGenerator<Student> studentGenerator,
        Faker faker) : base(options)
    {
        _mentorGenerator = mentorGenerator;
        _studentGenerator = studentGenerator;
        _faker = faker;
    }

    protected override IsuUserAssociation Generate(int index)
    {
        var user = _mentorGenerator.GeneratedEntities
            .Concat<User>(_studentGenerator.GeneratedEntities)
            .Skip(index)
            .FirstOrDefault();

        if (user is null)
            throw new IndexOutOfRangeException("Isu association count is greater than count of users.");

        var id = _faker.Random.Int(MinIsuNumber, MaxIsuNumber);
        var association = new IsuUserAssociation(user, id);

        user.AddAssociation(association);

        return association;
    }
}