using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Commands;

internal static class UpdateStudyGroup
{
    public record Command(Guid Id, string NewName) : IRequest<Response>;

    public record Response(StudyGroupDto Group);
}