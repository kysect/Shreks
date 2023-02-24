using System.Net;

namespace ITMO.Dev.ASAP.WebApi.Sdk.Exceptions;

public class RequestFailedException : AsapSdkException
{
    private RequestFailedException(string message, HttpStatusCode code, string content)
        : base(message)
    {
        Code = code;
        Content = content;
    }

    public HttpStatusCode Code { get; }

    public string Content { get; }

    internal static RequestFailedException Create(string message, HttpStatusCode code, string content)
    {
        return new RequestFailedException($"[{code}] Request failed: {message}", code, content);
    }
}