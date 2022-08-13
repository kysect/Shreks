using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using MediatR;

namespace Kysect.Shreks.Playground.Github.StubHandlers;

public class CreateSubmissionHandler : IRequestHandler<CreateSubmissionCommand.Command, CreateSubmissionCommand.Response>
{
    internal static readonly Submission _submission = new Submission(
        new Student("Lipa", "", "", new StudentGroup("luchshaya-gruppa")),
        new Assignment("laba", "lab1", new Points(0.0), new Points(100.0)), 
        DateTime.Now, "");

    public Task<CreateSubmissionCommand.Response> Handle(CreateSubmissionCommand.Command request, CancellationToken cancellationToken)
    {
        _submission.Payload = request.SubmissionUrl;
        _submission.SubmissionDateTime = DateTime.Now;

        return Task.FromResult(new CreateSubmissionCommand.Response(_submission.Id));
    }
}