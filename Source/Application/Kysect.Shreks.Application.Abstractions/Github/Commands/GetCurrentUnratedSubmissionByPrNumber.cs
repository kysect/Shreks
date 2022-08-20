using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetCurrentUnratedSubmissionByPrNumber
{
    public record Query(string organisation, string repository, long PrNumber) : IRequest<Response>;

    public record Response(SubmissionDto SubmissionDto);
}