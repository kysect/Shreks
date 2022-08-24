using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.DataAccess.Abstractions;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.InvalidateSubmission;

namespace Kysect.Shreks.Application.Handlers.Submissions;

internal class InvalidateSubmissionHandler : SubmissionStateHandlerBase<Command, Response>
{
    public InvalidateSubmissionHandler(IShreksDatabaseContext context, IMapper mapper) : base(context, mapper) { }

    protected override Guid GetSubmissionId(Command request)
        => request.SubmissionId;

    protected override Guid GetUserId(Command request)
        => request.UserId;

    protected override SubmissionState GetSubmissionState()
        => SubmissionState.Invalid;

    protected override Response Wrap(SubmissionDto submission)
        => new Response(submission);
}