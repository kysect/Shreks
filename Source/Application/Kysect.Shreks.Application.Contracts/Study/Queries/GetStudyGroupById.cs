using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

public static class GetStudyGroupById
{
    public record Query(Guid Id) : IRequest<Response>;

    public record Response(StudyGroupDto Group);
}