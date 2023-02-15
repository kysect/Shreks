using System.Net;

namespace ITMO.Dev.ASAP.WebApi.Sdk.Exceptions;

public class RequestFailedException : AsapSdkException
{
    private RequestFailedException(string message, HttpStatusCode code)
        : base(message)
    {
        Code = code;
    }

    public HttpStatusCode Code { get; }

    internal static RequestFailedException Create(string message, HttpStatusCode code)
    {
        return new RequestFailedException($"[{code}] Request failed: {message}", code);
    }
}