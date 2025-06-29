using EventCoalTrain.EventHandling;

namespace EventCoalTrain.EventStructure;

public sealed class EventKey<TPayload> : IEventKey
{
    public string Name { get; }

    private EventKey(string name)
    {
        Name = name;
    }
    
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
    
    public override string ToString() => $"EventKey<{typeof(TPayload).Name}>: {Name}";
    
    public override bool Equals(object? obj)
        => obj is EventKey<TPayload> other && other.Name == Name;

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
    
    public static void ClearRegistryForTesting()
    {
        #if DEBUG
            lock (EventBus.RegisteredKeys)
            {
                EventBus.RegisteredKeys.Clear();
            }
        #endif
    }
}