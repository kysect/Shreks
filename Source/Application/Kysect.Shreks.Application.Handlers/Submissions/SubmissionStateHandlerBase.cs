using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Exceptions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;

namespace Kysect.Shreks.Application.Handlers.Submissions;

public abstract class SubmissionStateHandlerBase<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
{
    private readonly IMapper _mapper;
    protected readonly IShreksDatabaseContext Context;

    protected SubmissionStateHandlerBase(IShreksDatabaseContext context, IMapper mapper)
    {
        Context = context;
        _mapper = mapper;
    }

    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var submissionId = GetSubmissionId(request);
        var userId = GetUserId(request);

        var submission = await Context.Submissions.GetByIdAsync(submissionId, cancellationToken);

        if (!submission.Student.User.Id.Equals(userId) &&
            !submission.GroupAssignment.Assignment.SubjectCourse.Mentors.Any(x => x.Id.Equals(userId)))
        {
            const string message = "User is not authorized to activate this submission";
            throw new UnauthorizedException(message);
        }

        submission.State = GetSubmissionState();
        await Context.SaveChangesAsync(cancellationToken);

        var submissionDto = _mapper.Map<SubmissionDto>(submission);

        return Wrap(submissionDto);
    }

    protected abstract Guid GetSubmissionId(TCommand request);
    protected abstract Guid GetUserId(TCommand request);
    protected abstract SubmissionState GetSubmissionState();

    protected abstract TResponse Wrap(SubmissionDto submission);
}