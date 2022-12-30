using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Commands;

internal class AddFractionDeadlinePolicy
{
    public record Command(Guid SubjectCourseId, TimeSpan SpanBeforeActivation, double Fraction) : IRequest;
}