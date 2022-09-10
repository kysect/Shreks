using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public Submission Submission { get; }

    public SubmissionContext(ILogger log, Guid issuerId, Submission submission) 
        : base(log, issuerId)
    {
        Submission = submission;
    }
}