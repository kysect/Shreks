using AutoMapper;
using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.CreateSubmissionCommand;
namespace Kysect.Shreks.Application.Handlers.Submissions;


public class CreateSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public CreateSubmissionHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var student = await _context.Students.GetByIdAsync(request.StudentId, cancellationToken);
        var assignment = await _context.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);

        if (!int.TryParse(request.Payload, out int pullRequestNumber))
            throw new DomainInvalidOperationException($"Cannot parse {request.Payload} to pull request number.");

        var submission = new Submission(student, assignment, DateOnly.FromDateTime(DateTime.Now));
        submission.UpdateAssociation(new GithubPullRequestSubmissionAssociation(submission, pullRequestNumber));
        await _context.Submissions.AddAsync(submission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}