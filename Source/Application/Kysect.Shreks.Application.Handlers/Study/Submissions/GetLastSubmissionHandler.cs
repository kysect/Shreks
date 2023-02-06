using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Submissions.Queries.GetLastSubmission;

namespace Kysect.Shreks.Application.Handlers.Study.Submissions;

internal class GetLastSubmissionHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetLastSubmissionHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Submission? submission = await _context.Submissions
            .Where(x => x.Student.UserId.Equals(request.StudentId))
            .Where(x => x.GroupAssignment.Assignment.Id.Equals(request.AssignmentId))
            .OrderByDescending(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null)
        {
            string message = $"Submission for student {request.StudentId} and assignment {request.AssignmentId} not found";
            throw new EntityNotFoundException(message);
        }

        return new Response(submission.ToDto());
    }
}