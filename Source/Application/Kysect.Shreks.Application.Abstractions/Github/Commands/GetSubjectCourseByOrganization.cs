using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetCurrentUnratedSumbissionByPrNumber
{
    public record Query(int PrNumber) : IRequest<Response>;

    public record Response(Guid SubmissionId);
}