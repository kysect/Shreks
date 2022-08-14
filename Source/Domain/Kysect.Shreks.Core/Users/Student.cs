using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Users;

public partial class Student : IEntity<Guid>
{
    public Student(User user, StudentGroup group) : this(Guid.NewGuid())
    {
        User = user;
        Group = group;
    }

    public virtual User User { get; protected init; }
    public virtual StudentGroup Group { get; protected init; }
}