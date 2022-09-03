using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.UserAssociations;

public abstract partial class UserAssociation : IEntity<Guid>
{
    protected UserAssociation(User user) : this(Guid.NewGuid())
    {
        User = user;
        user.AddAssociation(this);
    }

    public virtual User User { get; protected init; }
}