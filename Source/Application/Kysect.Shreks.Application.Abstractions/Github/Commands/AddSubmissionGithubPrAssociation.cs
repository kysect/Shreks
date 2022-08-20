using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Commands;

public static class AddSubmissionGithubPrAssociation
{
    public record Command(Guid SubmissionId, string Organisation, string Repository, long PrNumber) : IRequest;
}