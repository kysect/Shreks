using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Queries;

public static class GetStudyGroupById
{
    public record Query(Guid Id) : IRequest<Response>;

    public record Response(StudyGroupDto Group);
}