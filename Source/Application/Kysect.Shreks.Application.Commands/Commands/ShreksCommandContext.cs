using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Commands;

public class ShreksCommandContext
{
    public User Issuer { get; }
    
    public Submission? Submission { get; }
}