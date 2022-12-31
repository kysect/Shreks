namespace Kysect.Shreks.WebApi.Abstractions.Models.SubjectCourses;

public record AddFractionPolicyRequest(TimeSpan SpanBeforeActivation, double Fraction);