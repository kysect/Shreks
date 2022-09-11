using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public Submission Submission { get; }
    public ISubmissionService SubmissionService { get; set; }

    public SubmissionContext(Guid issuerId, Submission submission, ISubmissionService submissionService) 
        : base(issuerId)
    {
        Submission = submission;
        SubmissionService = submissionService;
    }
}