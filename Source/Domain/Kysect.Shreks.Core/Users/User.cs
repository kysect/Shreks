using Ardalis.Result;
using Kysect.Shreks.Core.UserAssociations;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Users;

public partial class User : IEntity<Guid>
{
    private readonly List<UserAssociation> _associations;

    public User(string firstName, string middleName, string lastName)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(middleName);
        ArgumentNullException.ThrowIfNull(lastName);

        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;

        _associations = new List<UserAssociation>();
    }

    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    public virtual IReadOnlyCollection<UserAssociation> Associations => _associations.AsReadOnly();

    public string FullName => $"{FirstName} {MiddleName} {LastName}";

    public override string ToString()
        => FullName;

    public Result AddAssociation(UserAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (_associations.Contains(association))
            return Result.Error($"User {this} already has association {association}");

        _associations.Add(association);
        return Result.Success();
    }

    public Result RemoveAssociation(UserAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Contains(association))
            return Result.Error($"User {this} does not have association {association}");

        if (!_associations.Remove(association))
            return Result.Error($"User {this} could not remove association {association}");

        return Result.Success();
    }
}