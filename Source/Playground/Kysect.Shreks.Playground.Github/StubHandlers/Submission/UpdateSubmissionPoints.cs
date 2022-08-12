using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Core.Study;
using MediatR;

namespace Kysect.Shreks.Playground.Github.StubHandlers;

public class UpdateSubmissionPointsHandler : IRequestHandler<UpdateSubmissionPoints.Command>
{
    public Task<Unit> Handle(UpdateSubmissionPoints.Command request, CancellationToken cancellationToken)
    {
        var submission = CreateSubmissionHandler._submission;
        if (submission.Id != request.SubmissionId)
        {
            throw new Exception("Submission not found");
        }
        submission.Points = request.NewPoints;
        return Unit.Task;
    }
}