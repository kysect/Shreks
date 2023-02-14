using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Queries.GetSubmissionByCode;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;

internal class GetSubmissionByCodeHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetSubmissionByCodeHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Submission? submission = await _context.Submissions
            .Where(x => x.Student.UserId.Equals(request.StudentId))
            .Where(x => x.GroupAssignment.Assignment.Id.Equals(request.AssignmentId))
            .Where(x => x.Code.Equals(request.Code))
            .SingleOrDefaultAsync(cancellationToken);

        return submission is null
            ? throw new EntityNotFoundException($"Couldn't find submission with code ${request.Code}")
            : new Response(submission.ToDto());
    }
}