namespace Kysect.Shreks.WebApi.Sdk.Exceptions;

public class UnauthorizedException : ShreksSdkException
{
    public UnauthorizedException(string message = "")
        : base(message) { }
}