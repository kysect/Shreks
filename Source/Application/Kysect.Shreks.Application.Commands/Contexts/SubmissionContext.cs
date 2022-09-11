using Kysect.Shreks.Application.Commands.Processors;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public Guid SubmissionId { get; set; }
    public ISubmissionService SubmissionService { get; set; }

    public SubmissionContext(Guid issuerId, Guid submissionId, ISubmissionService submissionService) 
        : base(issuerId)
    {
        SubmissionService = submissionService;
        SubmissionId = submissionId;
    }
}