using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;
using System.Text;

namespace Kysect.Shreks.Core.Users;

public partial class Student : IEntity
{
    public Student(User user, StudentGroup group) : this(user.Id)
    {
        User = user;
        Group = group;

        group.AddStudent(this);
    }

    [KeyProperty] public virtual User User { get; protected init; }

    public virtual StudentGroup? Group { get; protected set; }

    public void DismissFromStudyGroup()
    {
        if (Group is null)
            throw new DomainInvalidOperationException("Student is not in any group");

        Group = null;
    }

    public void TransferToAnotherGroup(StudentGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        group.AddStudent(this);
        Group?.RemoveStudent(this);

        Group = group;
    }

    public override string ToString()
    {
        var builder = new StringBuilder($"{User.FirstName} {User.LastName}");

        if (Group is not null)
            builder.Append($" from {Group.Name} ({UserId})");

        return builder.ToString();
    }
}