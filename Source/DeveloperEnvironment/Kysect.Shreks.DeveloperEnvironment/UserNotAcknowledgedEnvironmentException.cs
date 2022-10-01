namespace Kysect.Shreks.DeveloperEnvironment;

public class UserNotAcknowledgedEnvironmentException : InvalidOperationException
{
    public UserNotAcknowledgedEnvironmentException()
        : base("You must ensure that is it a right environment to execute SeedTestData command")
    {
    }
}