using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.UserAssociations;

public partial class IsuUserAssociation : UserAssociation
{
    public IsuUserAssociation(Guid id, User user, int universityId) : base(id, user)
    {
        UniversityId = universityId;
    }

    public int UniversityId { get; set; }
}