#define EVENTCOALTRAIN
using System;
using EventCoalTrain.EventHandling;

namespace EventCoalTrain.EventStructure;

/// <summary>
/// Represents a strongly-typed event key for use with the event bus.
/// Ensures unique registration of event keys by name.
/// Event keys associated with a packet must carry a payload with a specific type,
/// while event keys associated with a notification do not require a payload and can thus
/// be registered with a payload type of <see cref="Unit"/>.
/// </summary>
/// <typeparam name="TPayload">The type of the event payload.</typeparam>
public sealed class EventKey<TPayload> : IEventKey
{
    /// <summary>
    /// Gets the name of the event key.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventKey{TPayload}"/> class.
    /// </summary>
    /// <param name="name">The name of the event key.</param>
    private EventKey(string name)
    {
        Name = name;
    }
    
    /// <summary>
    /// Factory method that creates and registers a new event key with the specified name.
    /// Throws if the name is null, whitespace, or already registered.
    /// </summary>
    /// <param name="name">The name of the event key.</param>
    /// <returns>A new <see cref="EventKey{TPayload}"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if name is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the name is already registered.</exception>
    public static EventKey<TPayload> Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Event key name cannot be null or whitespace.", nameof(name));

        lock (EventBus.RegisteredKeys)
        {
            if (!EventBus.RegisteredKeys.Add(name))
                throw new InvalidOperationException($"Event key '{name}' is already registered.");
        }

        return new EventKey<TPayload>(name);
    }
    
    /// <summary>
    /// Returns a string representation of the event key.
    /// </summary>
    /// <returns>A string describing the event key.</returns>
    public override string ToString() => $"EventKey<{typeof(TPayload).Name}>: {Name}";
    
    /// <summary>
    /// Determines whether the specified object is equal to the current event key.
    /// </summary>
    /// <param name="obj">The object to compare with the current event key.</param>
    /// <returns>True if the specified object is equal to the current event key; otherwise, false.</returns>
    public override bool Equals(object obj)
        => obj is EventKey<TPayload> other && other.Name == Name;

    /// <summary>
    /// Returns a hash code for the event key.
    /// </summary>
    /// <returns>A hash code for the event key.</returns>
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
    
    /// <summary>
    /// Clears the event key registry for testing purposes.
    /// Only available in DEBUG builds.
    /// </summary>
    #if DEBUG
    public static void ClearRegistryForTesting()
    {
            lock (EventBus.RegisteredKeys)
            {
                EventBus.RegisteredKeys.Clear();
            }
    }
    #endif
}