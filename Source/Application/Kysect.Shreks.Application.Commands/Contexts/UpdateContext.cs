using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class UpdateContext : BaseContext
{
    public ISubmissionService SubmissionService { get; set; }
    public Assignment Assignment { get; set; }
    public User Student { get; set; }

    public UpdateContext(Guid issuerId, User student, Assignment assignment, ISubmissionService submissionService) :
        base(issuerId)
    {
        SubmissionService = submissionService;
        Assignment = assignment;
        Student = student;
    }
}