using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Abstractions.Submissions;

public interface ISubmissionFactory
{
    Task<Submission> CreateAsync(Guid userId, Guid assignmentId, CancellationToken cancellationToken);
}