using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Submissions.Queries.GetSubmissionByCode;

namespace Kysect.Shreks.Application.Handlers.Submissions;

internal class GetSubmissionByCodeHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubmissionByCodeHandler(IShreksDatabaseContext context)
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

        if (submission is null)
            throw new EntityNotFoundException($"Couldn't find submission with code ${request.Code}");

        return new Response(submission.ToDto());
    }
}