using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.CreateOrUpdateSubmissionCommand;

namespace Kysect.Shreks.Application.Handlers.Submissions;

public class CreateOrUpdateSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public CreateOrUpdateSubmissionHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Submission? submission = await _context.SubmissionAssociations
            .OfType<GithubPullRequestSubmissionAssociation>()
            .Where(a =>
                a.Organization == request.Organization
                && a.Repository == request.Repository
                && a.PullRequestNumber == request.PrNumber)
            .OrderByDescending(a => a.Submission.SubmissionDate)
            .Select(a => a.Submission)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null || submission.IsRated)
        {
            Student student = await _context.Students.GetByIdAsync(request.StudentId, cancellationToken);
            GroupAssignment groupAssignment = await _context.GroupAssignments
                .SingleAsync(ga => ga.AssignmentId == request.AssignmentId && ga.GroupId == student.Group.Id, cancellationToken);

            submission = new Submission(student, groupAssignment, DateOnly.FromDateTime(DateTime.Now), request.Payload);

            await _context.Submissions.AddAsync(submission, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            submission.SubmissionDate = DateOnly.FromDateTime(DateTime.Now);

            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}