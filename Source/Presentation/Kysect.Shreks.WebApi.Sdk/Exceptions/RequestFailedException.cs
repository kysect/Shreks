using System.Net;

namespace Kysect.Shreks.WebApi.Sdk.Exceptions;

public class RequestFailedException : ShreksSdkException
{
    private RequestFailedException(string message, HttpStatusCode code) : base(message)
    {
        Code = code;
    }

    internal static RequestFailedException Create(string message, HttpStatusCode code)
    {
        return new RequestFailedException($"[{code}] Request failed: {message}", code);
    }

    public HttpStatusCode Code { get; }
}