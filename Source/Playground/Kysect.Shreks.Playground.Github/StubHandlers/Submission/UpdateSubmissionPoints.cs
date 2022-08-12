using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Core.Study;
using MediatR;

namespace Kysect.Shreks.Playground.Github.StubHandlers;

public class UpdateSubmissionPointsHandler : IRequestHandler<UpdateSubmissionPoints.Command>
{
    private readonly Dictionary<Guid, Submission> _submissions;

    public UpdateSubmissionPointsHandler(Dictionary<Guid, Submission> submissions)
    {
        _submissions = submissions;
    }

    public Task<Unit> Handle(UpdateSubmissionPoints.Command request, CancellationToken cancellationToken)
    {
        var submission = _submissions.GetValueOrDefault(request.SubmissionId) ??
                         throw new Exception("Submission not found");
        submission.Points = request.NewPoints;
        return Unit.Task;
    }
}