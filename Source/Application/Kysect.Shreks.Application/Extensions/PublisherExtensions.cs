// ReSharper disable once CheckNamespace

namespace MediatR;

public static class PublisherExtensions
{
    // Extensions for proper copilot autocompletion
    public static Task PublishAsync(
        this IPublisher publisher,
        INotification notification,
        CancellationToken cancellationToken)
    {
        return publisher.Publish(notification, cancellationToken);
    }
}