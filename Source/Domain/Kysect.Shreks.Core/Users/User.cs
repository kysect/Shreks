using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Users;

public partial class User : IEntity<Guid>
{
    private readonly HashSet<UserAssociation> _associations;

    public User(string firstName, string middleName, string lastName)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(middleName);
        ArgumentNullException.ThrowIfNull(lastName);

        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;

        _associations = new HashSet<UserAssociation>();
    }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public virtual IReadOnlyCollection<UserAssociation> Associations => _associations;

    public override string ToString()
    {
        return $"{FirstName} {MiddleName} {LastName}";
    }

    public void AddAssociation(UserAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        Type associationType = association.GetType();

        if (Associations.Any(a => a.GetType() == associationType))
            throw new DomainInvalidOperationException($"User {this} already has {associationType} association");

        _associations.Add(association);
    }

    public void RemoveAssociation(UserAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Remove(association))
            throw new DomainInvalidOperationException($"User {this} could not remove association {association}");
    }

    public bool HasAssociation<T>() where T : UserAssociation
    {
        return Associations.Any(a => a is T);
    }

    public T? FindAssociation<T>() where T : UserAssociation
    {
        return Associations.OfType<T>().SingleOrDefault();
    }
}