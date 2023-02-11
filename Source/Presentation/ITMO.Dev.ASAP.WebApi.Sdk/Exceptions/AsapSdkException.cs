namespace ITMO.Dev.ASAP.WebApi.Sdk.Exceptions;

public class AsapSdkException : Exception
{
    private protected AsapSdkException(string message)
        : base(message) { }
}