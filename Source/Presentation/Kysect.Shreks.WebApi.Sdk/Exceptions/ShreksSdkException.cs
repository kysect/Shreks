namespace Kysect.Shreks.WebApi.Sdk.Exceptions;

public class ShreksSdkException : Exception
{
    private protected ShreksSdkException(string message)
        : base(message) { }
}