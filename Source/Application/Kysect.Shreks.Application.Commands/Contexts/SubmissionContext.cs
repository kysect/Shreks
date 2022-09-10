using Kysect.Shreks.Application.Dto.Study;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public SubmissionDto Submission { get; }

    public SubmissionContext(ILogger log, Guid issuerId, SubmissionDto submission) 
        : base(log, issuerId)
    {
        Submission = submission;
    }
}