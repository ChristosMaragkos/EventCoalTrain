using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventSource
{
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
        /// Gets the name of the event from the key.
        /// </summary>
        public string Name => Key.Name;

        /// <summary>
        /// Initializes a new descriptor-like instance of the <see cref="Packet{TPayload}"/> class that only carries the key.
        /// Use <c>EventBus.Publish(packet, payload)</c> to publish with a fresh payload per call.
        /// </summary>
        /// <param name="key">The event key.</param>
        public Packet(EventKey<TPayload> key)
        {
            Key = key;
            // Payload intentionally left at default; use Publish(packet, payload) overload for sending data.
            // This enables safely caching packets as descriptors.
        }
    }
}