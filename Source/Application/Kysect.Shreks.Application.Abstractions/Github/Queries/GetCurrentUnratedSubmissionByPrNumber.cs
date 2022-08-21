using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetCurrentUnratedSubmissionByPrNumber
{
    public record Query(string Organization, string Repository, long PrNumber) : IRequest<Response>;

    public record Response(SubmissionDto SubmissionDto);
}