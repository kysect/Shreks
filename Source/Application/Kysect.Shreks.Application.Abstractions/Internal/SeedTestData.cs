using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Internal;

public static class SeedTestData
{
    public record Query(string Environment) : IRequest<Unit>;

    public class UserNotAcknowledgedEnvironmentException : InvalidOperationException
    {
        public UserNotAcknowledgedEnvironmentException()
            : base("You must ensure that is it a right environment to execute SeedTestData command")
        {
        }
    }
}