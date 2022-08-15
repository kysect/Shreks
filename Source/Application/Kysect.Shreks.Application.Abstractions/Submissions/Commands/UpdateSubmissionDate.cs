using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Commands;

public static class UpdateSubmissionDate
{
    public record Command(Guid SubmissionId, DateTime NewDate) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}