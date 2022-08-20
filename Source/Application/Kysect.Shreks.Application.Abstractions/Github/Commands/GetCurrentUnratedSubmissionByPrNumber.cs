using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetCurrentUnratedSubmissionByPrNumber
{
    public record Query(string organisation, string repository, int PrNumber) : IRequest<Response>;

    public record Response(Guid SubmissionId);
}