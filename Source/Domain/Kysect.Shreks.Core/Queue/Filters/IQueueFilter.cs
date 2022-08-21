using Submission = Kysect.Shreks.Core.Submissions.Submission;

namespace Kysect.Shreks.Core.Queue.Filters;

public interface IQueueFilter
{
    IQueryable<Submission> Filter(IQueryable<Submission> query);
}