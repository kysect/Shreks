using Kysect.Shreks.Common.Exceptions.User;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.UserAssociations;

public partial class IsuUserAssociation : UserAssociation
{
    public IsuUserAssociation(User user, int universityId) : base(user)
    {
        if (user.HasAssociation<IsuUserAssociation>())
            throw new UserAlreadyHasAssociationException("isu");
        
        UniversityId = universityId;
        user.AddAssociation(this);
    }

    public int UniversityId { get; set; }
}