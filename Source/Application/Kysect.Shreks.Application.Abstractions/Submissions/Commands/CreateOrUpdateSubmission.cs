using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Commands;

public static class CreateOrUpdateSubmissionCommand
{
    public record Command(Guid StudentId, Guid AssignmentId, string Payload, string Organization, string Repository, int PrNumber) : IRequest<Response>;

    public record Response(SubmissionDto Submission);
}