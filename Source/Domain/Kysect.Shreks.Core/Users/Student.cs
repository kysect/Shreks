using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.UserAssociations;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Users;

public partial class Student : IEntity
{
    public Student(User user, StudentGroup group) : this(userId: user.Id)
    {
        User = user;
        Group = group;
    }

    [KeyProperty]
    public virtual User User { get; protected init; }

    public virtual StudentGroup Group { get; protected init; }

    public override string ToString()
        => $"{User.FirstName} {User.LastName} from {Group.Name} ({UserId})";
}