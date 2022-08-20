using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Commands;

public static class AddSubmissionGithubPrAssociation
{
    public record Command(Guid SubmissionId, int PrNumber) : IRequest;
}