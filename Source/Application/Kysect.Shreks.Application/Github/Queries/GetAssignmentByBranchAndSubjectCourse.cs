using System.Net.NetworkInformation;
using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Github.Queries;

public static class GetAssignmentByBranchAndSubjectCourse
{
    public record Query(string BranchName, Guid SubjectCourseId) : IRequest<Response>;

    public record Response(Guid AssignmentId);

    public class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly IShreksDatabaseContext _context;

        public QueryHandler(IShreksDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var subjectCourse = await _context.SubjectCourses.FirstOrDefaultAsync(
                sc => sc.Id == request.SubjectCourseId,
                cancellationToken);

            if (subjectCourse is null)
                throw new EntityNotFoundException($"SubjectCourse with id {request.SubjectCourseId} not found");
            
            throw new NotImplementedException();
            
            // TODO: Add logic to get assignment by branch name
            // a => request.BranchName.StartsWith(a.ShortTitle)
            var assignment = subjectCourse.Assignments.FirstOrDefault();
            
            if (assignment is null)
                throw new EntityNotFoundException($"Assignment with name {request.BranchName} not found");
            
            return new Response(assignment.Id);
        }
    }
}