using System;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventHandling
{
    public interface IEventBus
    {
        // Subscribe (disposable patterns)
        IDisposable Subscribe<TPayload>(Packet<TPayload> packet, Action<TPayload> handler);
        IDisposable Subscribe(Notification notification, Action handler);

        // Publish
        void Publish<TPayload>(Packet<TPayload> packet, TPayload payload);
        void Publish(Notification notification);

        // Bulk ops / queries
        void UnsubscribeAll(IEventKey key);
        bool HasSubscribers(IEventKey key);
        int Count(IEventKey key);
        void Clear();

        /// Error callback on publish exceptions
        event Action<Exception, IEventKey, Delegate> OnPublishError;
    }
}
