using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.Study;

public record struct StudentAssignmentPoints(
    Student Student,
    Assignment Assignment,
    Points Points,
    DateOnly SubmissionDate);