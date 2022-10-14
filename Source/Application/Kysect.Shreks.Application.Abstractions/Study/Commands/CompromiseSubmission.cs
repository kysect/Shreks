using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Commands;

public static class CompromiseSubmission
{
    public record struct Command(Guid SubmissionId) : IRequest;
}