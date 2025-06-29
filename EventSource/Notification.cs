using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventSource;

public sealed class Notification : IEvent
{
    public EventKey<Unit> Key { get; }

    public string Name => Key.Name;

    public Notification(EventKey<Unit> key)
    {
        Key = key;
    }
}