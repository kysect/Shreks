using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands;

internal class AddFractionDeadlinePolicy
{
    public record Command(Guid SubjectCourseId, TimeSpan SpanBeforeActivation, double Fraction) : IRequest;
}