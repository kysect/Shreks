using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Commands;

public class ShreksCommandContext
{
    public ShreksCommandContext(User issuer, Submission? submission)
    {
        Issuer = issuer;
        Submission = submission;
    }

    public User Issuer { get; }
    
    public Submission? Submission { get; }
}