using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Users;

public partial class Student : IEntity<Guid>
{
    // TODO: WI-227
    public Student(User user, StudentGroup group) : this(user.Id)
    {
        User = user;
        Group = group;
    }

    public virtual User User { get; protected init; }
    public virtual StudentGroup Group { get; protected init; }

    public override string ToString() => $"{User.FirstName} {User.LastName} from {Group.Name} ({Id})";
}