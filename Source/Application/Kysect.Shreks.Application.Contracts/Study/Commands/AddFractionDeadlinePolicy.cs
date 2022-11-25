using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Commands;

public class AddFractionDeadlinePolicy
{
    public record Command(Guid SubjectCourseId, TimeSpan SpanBeforeActivation, double Fraction) : IRequest<Response>;
    public record Response();
}