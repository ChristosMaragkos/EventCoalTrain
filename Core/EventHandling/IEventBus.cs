using System;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventHandling;

public interface IEventBus
{
    // Subscribe (disposable patterns)
    IDisposable Subscribe<TPayload>(EventKey<TPayload> key, Action<TPayload> handler);
    IDisposable Subscribe(Notification notification, Action handler);

    // Legacy-shaped operations (void-based)
    void Unsubscribe<TPayload>(EventKey<TPayload> key, Action<TPayload> handler);
    void Unsubscribe(Notification notification, Action handler);

    // Publish
    void Publish<TPayload>(Packet<TPayload> packet);
    void Publish(Notification notification);

    // Bulk ops / queries
    void UnsubscribeAll(IEventKey key);
    bool HasSubscribers(IEventKey key);
    int Count(IEventKey key);
    void Clear();

    // Error callback on publish exceptions
    event Action<Exception, IEventKey, Delegate>? OnPublishError;
}
