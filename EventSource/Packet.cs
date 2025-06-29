using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventSource;

public class Packet<TPayload> : IEvent
{
    public EventKey<TPayload> Key { get; }
    public TPayload Payload { get; }
    
    public string Name => Key.Name;

    public Packet(EventKey<TPayload> key, TPayload payload)
    {
        Key = key;
        Payload = payload;
    }
}