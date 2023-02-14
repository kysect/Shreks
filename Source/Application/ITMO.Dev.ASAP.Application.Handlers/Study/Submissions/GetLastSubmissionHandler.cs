using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Queries.GetLastSubmission;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;

internal class GetLastSubmissionHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetLastSubmissionHandler(IDatabaseContext context)
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