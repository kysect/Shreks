using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Abstractions.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Submissions.Queries.GetSubmissionDeadline;

namespace Kysect.Shreks.Application.Handlers.Submissions;

public class GetSubmissionDeadlineHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubmissionDeadlineHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<Submission> submissionQuery = _context.Submissions
            .Where(x => x.Id.Equals(request.SubmissionId));

        IQueryable<Student> studentsQuery = submissionQuery
            .Select(x => x.Student);

        IQueryable<StudentGroup> groupQuery = _context.StudentGroups
            .Where(x => x.Students.Any(xx => studentsQuery.Contains(xx)));

        IQueryable<Assignment> assignmentQuery = submissionQuery
            .Select(x => x.Assignment);

        IQueryable<GroupAssignment> groupAssignmentsQuery = _context.GroupAssignments
            .Where(x => groupQuery.Contains(x.Group) && assignmentQuery.Contains(x.Assignment));

        var groupAssignment = await groupAssignmentsQuery.SingleOrDefaultAsync(cancellationToken);
        
        if (groupAssignment is null)
            throw new EntityNotFoundException("Group assignment for the given submission not found");

        return new Response(groupAssignment.Deadline);
    }
}