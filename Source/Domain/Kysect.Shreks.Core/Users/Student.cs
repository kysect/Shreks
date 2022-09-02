using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.UserAssociations;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Users;

public partial class Student : IEntity
{
    public Student(User user, StudentGroup group, int universityId) : this(userId: user.Id)
    {
        User = user;
        Group = group;

        user.AddAssociation(new IsuUserAssociation(user, universityId));
    }

    [KeyProperty]
    public virtual User User { get; protected init; }

    public virtual StudentGroup Group { get; protected init; }

    public int GetUniversityId()
    {
        IsuUserAssociation isuUserAssociation = User
            .Associations
            .OfType<IsuUserAssociation>()
            .First();

        return isuUserAssociation.UniversityId;
    }

    public override string ToString()
        => $"{User.FirstName} {User.LastName} from {Group.Name} ({UserId})";
}