using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Commands;

public static class UpdateSubmissionPayload
{
    public record Command(Guid SubmissionId, string NewPayload) : IRequest;
}