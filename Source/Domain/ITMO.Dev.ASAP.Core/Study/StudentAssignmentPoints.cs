using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.Study;

public record struct StudentAssignmentPoints(
    Student Student,
    Assignment Assignment,
    bool IsBanned,
    Points Points,
    DateOnly SubmissionDate);