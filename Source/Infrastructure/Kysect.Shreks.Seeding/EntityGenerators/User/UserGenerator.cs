using Bogus;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class UserGenerator : EntityGeneratorBase<User>
{
    private readonly Faker _faker;

    public UserGenerator(EntityGeneratorOptions<User> options, Faker faker)
        : base(options)
    {
        _faker = faker;
    }

    protected override User Generate(int index)
    {
        return new User(
            _faker.Random.Guid(),
            _faker.Name.FirstName(),
            _faker.Name.MiddleName(),
            _faker.Name.LastName());
    }
}