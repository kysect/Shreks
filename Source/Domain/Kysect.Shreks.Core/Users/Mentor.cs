using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Users;

public partial class Mentor : IEntity<Guid>
{
    public Mentor(User user) : this(user.Id)
    {
        User = user;
    }

    public virtual User User { get; protected init; }
}
