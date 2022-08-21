using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.CreateSubmissionCommand;
namespace Kysect.Shreks.Application.Handlers.Submissions;


public class CreateSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;
    private readonly IMapper _mapper;

    public CreateSubmissionHandler(IShreksDatabaseContext context, ITableUpdateQueue tableUpdateQueue, IMapper mapper)
    {
        _context = context;
        _tableUpdateQueue = tableUpdateQueue;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var student = await _context.Students.GetByIdAsync(request.StudentId, cancellationToken);
        GroupAssignment groupAssignment = await _context.GroupAssignments
            .SingleAsync(ga => ga.AssignmentId == request.AssignmentId && ga.GroupId == student.Group.Id, cancellationToken);

        var submission = new Submission(student, groupAssignment, DateOnly.FromDateTime(DateTime.Now), request.Payload);
        await _context.Submissions.AddAsync(submission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId());

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}