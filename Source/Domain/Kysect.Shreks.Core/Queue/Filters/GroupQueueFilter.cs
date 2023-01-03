using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public class GroupQueueFilter : IQueueFilter
{
    public GroupQueueFilter(IReadOnlyCollection<StudentGroup> groups)
    {
        Groups = groups;
    }

    public IReadOnlyCollection<StudentGroup> Groups { get; }

    public IQueryable<Submission> Filter(IQueryable<Submission> query)
    {
        return query.Where(s => Groups.Contains(s.Student.Group));
    }
}