using System;
using System.Collections.Generic;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventHandling;

/// <summary>
/// Provides a static event bus for subscribing, unsubscribing, and publishing events.
/// </summary>
public static class EventBus
{
    /// <summary>
    /// Stores event subscribers mapped by their event key.
    /// </summary>
    private static readonly Dictionary<IEventKey, List<Delegate>> Subscribers = new();
    
    /// <summary>
    /// Contains all registered event keys for reference.
    /// </summary>
    public static readonly HashSet<string> RegisteredKeys = new();

    /// <summary>
    /// Subscribes a handler to a packet event with a specific payload type.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="packet">The packet to subscribe to.</param>
    /// <param name="handler">The handler to invoke when the event is published.</param>
    /// <remarks>
    /// If you use an inline lambda to subscribe to an event,
    /// you will not be able to unsubscribe, as the lambda does not have a reference.
    /// If you plan to implement dynamic event subscription, use a method as a handler.
    /// </remarks>
    public static void Subscribe<TPayload>(Packet<TPayload> packet, Action<TPayload> handler)
    {
        if (!Subscribers.TryGetValue(packet.Key, out var list))
        {
            list = new List<Delegate>();
            Subscribers[packet.Key] = list;
        }
        
        list.Add(handler);
    }
    
    /// <summary>
    /// Subscribes a handler to a notification event.
    /// </summary>
    /// <param name="notification">The notification to subscribe to.</param>
    /// <param name="handler">The handler to invoke when the notification is published.</param>
    /// <remarks>
    /// If you use an inline lambda to subscribe to an event,
    /// you will not be able to unsubscribe, as the lambda does not have a reference.
    /// If you plan to implement dynamic event subscription, use a method as a handler.
    /// </remarks>
    public static void Subscribe(Notification notification, Action handler)
    {
        if (!Subscribers.TryGetValue(notification.Key, out var list))
        {
            list = new List<Delegate>();
            Subscribers[notification.Key] = list;
        }
        
        list.Add(handler);
    }

    /// <summary>
    /// Unsubscribes a handler from a packet event with a specific payload type.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="packet">The packet to unsubscribe from.</param>
    /// <param name="handler">The handler to remove.</param>
    /// <remarks>If you use an inline lambda to subscribe to an event,
    /// you will not be able to unsubscribe, as the lambda does not have a reference.
    /// If you plan to implement dynamic event subscription, make sure you use a method as a handler.</remarks>
    public static void Unsubscribe<TPayload>(Packet<TPayload> packet, Action<TPayload> handler)
    {
        if (Subscribers.TryGetValue(packet.Key, out var list))
        {
            list.Remove(handler);
        }
    }
    
    /// <summary>
    /// Unsubscribes a handler from a packet event with a specific payload type.
    /// </summary>
    /// <param name="notification">The type of the payload.</param>
    /// <param name="handler">The handler to remove.</param>
    /// <remarks>If you use an inline lambda to subscribe to an event,
    /// you will not be able to unsubscribe, as the lambda does not have a reference.
    /// If you plan to implement dynamic event subscription, make sure you use a method as a handler.</remarks>
    public static void Unsubscribe(Notification notification, Action handler)
    {
        if (Subscribers.TryGetValue(notification.Key, out var list))
        {
            list.Remove(handler);
        }
    }

    /// <summary>
    /// Publishes a packet event with a specific payload type to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload contained in the packet.</typeparam>
    /// <param name="packet">The packet containing the event key and payload to publish.</param>
    /// <remarks>
    /// Only handlers for the designated payload will be invoked. 
    /// If no handlers are subscribed for the given packet key, the method returns without action.
    /// </remarks>
    public static void Publish<TPayload>(Packet<TPayload> packet)
    {
        if (!Subscribers.TryGetValue(packet.Key, out var list)) return;
        
        foreach (var handler in list)
        {
            if (handler is Action<TPayload> action)
                action(packet.Payload);
        }
    }

    /// <summary>
    /// Publishes a notification event to all subscribed handlers.
    /// </summary>
    /// <param name="notification">The notification containing the event key to publish.</param>
    /// <remarks>
    /// Only handlers subscribed to the given notification key will be invoked.
    /// If no handlers are subscribed for the given notification key, the method returns without action.
    /// </remarks>
    public static void Publish(Notification notification)
    {
        if (!Subscribers.TryGetValue(notification.Key, out var list)) return;

        foreach (var handler in list)
            if (handler is Action action)
                action();
    }
}