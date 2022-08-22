using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public interface IQueueFilter
{
    IQueryable<Submission> Filter(IQueryable<Submission> query);
}