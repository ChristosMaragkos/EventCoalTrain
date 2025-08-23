#nullable enable
using System;
using System.Collections.Generic;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventHandling
{
    /// <summary>
    /// Static wrapper over a thread-safe event bus implementation.
    /// </summary>
    public static class EventBus
    {
        private static IEventBus _bus = new DefaultEventBus();

        /// <summary>
        /// Contains all registered event key names (used by EventKey.Of for uniqueness).
        /// </summary>
        public static readonly HashSet<string> RegisteredKeys = new HashSet<string>();

        /// <summary>
        /// Access the underlying event bus instance (supports IDisposable subscriptions).
        /// </summary>
        public static IEventBus Instance => _bus;

        /// <summary>
        /// Replace the underlying bus implementation.
        /// </summary>
        public static void Configure(IEventBus bus)
            => _bus = bus ?? throw new ArgumentNullException(nameof(bus));

        /// <summary>
        /// Raised when a subscriber throws during publish.
        /// </summary>
        public static event Action<Exception, IEventKey, Delegate>? OnPublishError
        {
            add => _bus.OnPublishError += value;
            remove => _bus.OnPublishError -= value;
        }
        
        public static void Subscribe<TPayload>(Packet<TPayload> packet, Action<TPayload> handler)
            => _bus.Subscribe(packet, handler);
        public static void Subscribe(Notification notification, Action handler)
            => _bus.Subscribe(notification, handler);
    
        // Publish
        public static void Publish<TPayload>(Packet<TPayload> packet, TPayload payload) => 
            _bus.Publish(packet, payload);
        public static void Publish(Notification notification) => _bus.Publish(notification);

        // Bulk operations
        public static void UnsubscribeAll(IEventKey key) => _bus.UnsubscribeAll(key);
        public static bool HasSubscribers(IEventKey key) => _bus.HasSubscribers(key);
        public static int Count(IEventKey key) => _bus.Count(key);
        public static void Clear() => _bus.Clear();
    }
}