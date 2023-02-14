namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourses;

public record AddFractionPolicyRequest(TimeSpan SpanBeforeActivation, double Fraction);