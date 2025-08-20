using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventSource;

/// <summary>
/// Represents a packet containing an event key and its associated payload.
/// Packets are used to notify subscribers about events while transferring data.
/// They can be used to send data between different parts of the application,
/// like a player scoring points with the payload being the score increase.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public class Packet<TPayload> : IEvent
{
    /// <summary>
    /// Gets the event key associated with this packet.
    /// </summary>
    public EventKey<TPayload> Key { get; }

    /// <summary>
    /// Gets the payload carried by this packet.
    /// </summary>
    public TPayload Payload { get; }
    
    /// <summary>
    /// Gets the name of the event from the key.
    /// </summary>
    public string Name => Key.Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="Packet{TPayload}"/> class.
    /// </summary>
    /// <param name="key">The event key.</param>
    /// <param name="payload">The payload to associate with the key.</param>
    public Packet(EventKey<TPayload> key, TPayload payload)
    {
        Key = key;
        Payload = payload;
    }
}