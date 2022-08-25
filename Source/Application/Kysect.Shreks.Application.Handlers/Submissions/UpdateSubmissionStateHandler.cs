using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Exceptions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;

using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.UpdateSubmissionState;

namespace Kysect.Shreks.Application.Handlers.Submissions;

public class UpdateSubmissionStateHandler : IRequestHandler<Command, Response>
{
    private readonly IMapper _mapper;
    protected readonly IShreksDatabaseContext Context;

    public UpdateSubmissionStateHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        Context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var submission = await Context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        if (!submission.Student.User.Id.Equals(request.UserId) &&
            !submission.GroupAssignment.Assignment.SubjectCourse.Mentors.Any(x => x.Id.Equals(request.UserId)))
        {
            const string message = "User is not authorized to activate this submission";
            throw new UnauthorizedException(message);
        }

        submission.State = _mapper.Map<SubmissionState>(request.State);
        await Context.SaveChangesAsync(cancellationToken);

        var submissionDto = _mapper.Map<SubmissionDto>(submission);
        return new Response(submissionDto);
    }
}