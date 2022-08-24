using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Users;

public partial class User : IEntity<Guid>
{
    private readonly HashSet<UserAssociation> _associations;

    public User(string firstName, string middleName, string lastName, string githubUsername)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(middleName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(githubUsername);

        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        GithubUsername = githubUsername;

        _associations = new HashSet<UserAssociation>();
    }

    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string GithubUsername { get; set; }

    public virtual IReadOnlyCollection<UserAssociation> Associations => _associations;
    
    public override string ToString()
        => $"{FirstName} {MiddleName} {LastName}";

    public void AddAssociation(UserAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Add(association))
            throw new DomainInvalidOperationException($"User {this} already has association {association}");
    }

    public void RemoveAssociation(UserAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Remove(association))
            throw new DomainInvalidOperationException($"User {this} could not remove association {association}");
    }
}