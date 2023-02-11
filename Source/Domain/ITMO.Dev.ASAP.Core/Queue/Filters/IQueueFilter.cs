using ITMO.Dev.ASAP.Core.Submissions;

namespace ITMO.Dev.ASAP.Core.Queue.Filters;

public interface IQueueFilter
{
    IQueryable<Submission> Filter(IQueryable<Submission> query);
}