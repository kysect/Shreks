using Kysect.CommonLib;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetAssignmentByBranchAndSubjectCourse;
namespace Kysect.Shreks.Application.Handlers.Github;

public class GetAssignmentByBranchAndSubjectCourseHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetAssignmentByBranchAndSubjectCourseHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var assignment = subjectCourse.Assignments.FirstOrDefault(a => a.ShortName == request.BranchName);

        if (assignment is null)
        {
            var branchName = request.BranchName;
            string assignments = subjectCourse
                .Assignments
                .OrderBy(a => a.Order)
                .ToSingleString(a => a.ShortName);

            var message = $"Assignment with branch name '{branchName}' for subject course '{subjectCourse.Title}' was not found." +
                          $"\nEnsure that branch name is correct. Available assignments: {assignments}";
            throw new EntityNotFoundException(message);
        }

        return new Response(assignment.Id);
    }
}