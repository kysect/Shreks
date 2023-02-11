namespace ITMO.Dev.ASAP.WebApi.Sdk.Exceptions;

public class UnauthorizedException : AsapSdkException
{
    public UnauthorizedException(string message = "")
        : base(message) { }
}