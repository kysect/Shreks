using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.UserAssociations;

public partial class IsuUserAssociation : UserAssociation
{
    public IsuUserAssociation(User user, int universityId) : base(user)
    {
        UniversityId = universityId;
    }

    public int UniversityId { get; protected init; }
}