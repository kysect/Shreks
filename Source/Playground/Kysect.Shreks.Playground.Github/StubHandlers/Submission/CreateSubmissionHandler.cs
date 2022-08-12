using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Core.Study;
using MediatR;

namespace Kysect.Shreks.Playground.Github.StubHandlers;

public class CreateSubmissionHandler : IRequestHandler<CreateSubmissionCommand.Command, CreateSubmissionCommand.Response>
{
    internal static readonly Submission _submission = new Submission(null!, null!, DateTime.Now, "");

    public Task<CreateSubmissionCommand.Response> Handle(CreateSubmissionCommand.Command request, CancellationToken cancellationToken)
    {
        _submission.Payload = request.SubmissionUrl;
        _submission.SubmissionDateTime = DateTime.Now;

        return Task.FromResult(new CreateSubmissionCommand.Response(_submission.Id));
    }
}