using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.UserAssociations;

public abstract partial class UserAssociation : IEntity<Guid>
{
    protected UserAssociation(Guid id, User user) : this(id)
    {
        User = user;
        user.AddAssociation(this);
    }

    public virtual User User { get; protected init; }
}