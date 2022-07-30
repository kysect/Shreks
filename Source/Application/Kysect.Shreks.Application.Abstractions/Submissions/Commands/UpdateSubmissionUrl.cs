using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Commands;

public static class UpdateSubmissionUrl
{
    public record Command(Guid SubmissionId, string NewUrl) : IRequest;
}