using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.Users;

public partial class Student : User
{
    public Student(string firstName, string middleName, string lastName, StudentGroup group)
        : base(firstName, middleName, lastName)
    {
        Group = group;
    }

    public virtual StudentGroup Group { get; protected init; }
}