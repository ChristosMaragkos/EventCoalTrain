# EventCoalTrain

Simple, typed event bus for .NET (Unity, Godot, and general C# apps).

Supported TFMs: netstandard2.1, net8.0.

## Features
- Strongly-typed event keys (EventKey<T>)
- Packets with payloads and Notifications without payloads
- Thread-safe subscriptions and publishing
- Snapshot iteration during publish (handlers can add/remove safely)
- Empty-list cleanup to avoid leaks
- Optional error callback for subscriber exceptions
- Static EventBus wrapper and an IEventBus interface with IDisposable subscriptions
- Global preprocessor symbol for consumers: EVENTCOALTRAIN

## Best practice: define record payloads and cache keys and packet descriptors
Model your domain events with record (or record struct) payloads for clarity and immutability, then create a typed EventKey based on that record. Cache and reuse:
- the EventKey<TPayload>
- an optional Packet<TPayload> descriptor constructed with the key only (no payload)
- Notification instances for no-payload events

Publish by supplying a fresh payload per call. This avoids stale state while allowing you to cache what’s stable.

```csharp
using EventCoalTrain.EventStructure;
using EventCoalTrain.EventSource;

// 1) Define the payload as a record/record struct
public readonly record struct ScoreIncreased(int Amount, string Source);

public static class Events
{
    // 2) Define and cache the event key typed on the payload record
    public static readonly EventKey<ScoreIncreased> ScoreIncreasedKey = EventKey<ScoreIncreased>.Of("ScoreIncreased");

    // 3) Optionally cache a packet descriptor (key only)
    public static readonly Packet<ScoreIncreased> ScoreIncreased = new Packet<ScoreIncreased>(ScoreIncreasedKey);

    // Notification-style event (no payload). Cache both key and notification instance.
    public static readonly EventKey<Unit> ButtonClickedKey = EventKey<Unit>.Of("ButtonClicked");
    public static readonly Notification ButtonClicked = new Notification(ButtonClickedKey);
}
```

- Subscribe using the cached keys/notifications via EventBus.Instance.
- Publish using either the cached packet descriptor or the key directly with a fresh payload instance each time.

## Quick start
```csharp
using EventCoalTrain.EventStructure;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventHandling;

// Subscribe (preferred instance API returning IDisposable)
var subScore = EventBus.Instance.Subscribe(Events.ScoreIncreasedKey, e => Console.WriteLine($"+{e.Amount} ({e.Source})"));
var subClick = EventBus.Instance.Subscribe(Events.ButtonClicked, () => Console.WriteLine("clicked"));

// Publish with a cached packet descriptor + fresh payload
EventBus.Publish(Events.ScoreIncreased, new ScoreIncreased(10, "LevelComplete"));

// Or publish with key + payload
EventBus.Publish(Events.ScoreIncreasedKey, new ScoreIncreased(5, "Combo"));

// Notification publish reuses cached notification
EventBus.Publish(Events.ButtonClicked);

// Unsubscribe via disposables
subScore.Dispose();
subClick.Dispose();
```

Notes:
- Cache and reuse EventKey<T> and optional Packet<T> descriptors; supply a new payload per publish. This is allocation-light (especially with record structs) and thread-safe.
- Prefer EventBus.Instance for subscriptions to get IDisposable handles.
- The static wrapper methods exist for convenience/back-compat; the instance API is recommended for lifecycle management.

## API overview

### Event keys, packet descriptors, and payload records
```csharp
public readonly record struct DamageTaken(int Amount, string Cause);

var DamageTakenKey = EventKey<DamageTaken>.Of("DamageTaken");
var DamageTakenPacket = new Packet<DamageTaken>(DamageTakenKey); // descriptor
var TickedKey = EventKey<Unit>.Of("Ticked");
var Ticked = new Notification(TickedKey);
```
- Equality and GetHashCode of EventKey include both Name and payload type to avoid cross-type collisions.
- Names must still be unique globally (across all payload types).

### Subscribing (instance, preferred)
```csharp
var sub1 = EventBus.Instance.Subscribe(DamageTakenKey, d => Console.WriteLine($"damage {d.Amount} from {d.Cause}"));
var sub2 = EventBus.Instance.Subscribe(Ticked, () => Console.WriteLine("ticked"));

// Unsubscribe
sub1.Dispose();
sub2.Dispose();
```

### Publishing
```csharp
// Using cached descriptor + fresh payload
EventBus.Publish(DamageTakenPacket, new DamageTaken(5, "Trap"));

// Or using key + payload
EventBus.Publish(DamageTakenKey, new DamageTaken(3, "Fall"));

EventBus.Publish(Ticked);
```

### Bulk operations
```csharp
EventBus.UnsubscribeAll(DamageTakenKey);
bool any = EventBus.HasSubscribers(DamageTakenKey);
int count = EventBus.Count(DamageTakenKey);
EventBus.Clear(); // remove all subscriptions
```

### Error handling
When a subscriber throws during publish, publishing continues for other subscribers. You can observe errors via a callback:
```csharp
EventBus.OnPublishError += (ex, key, del) =>
{
    Console.Error.WriteLine($"Subscriber threw for {key.Name}: {ex}");
};
```

## Thread-safety and iteration
- The internal subscribers map is guarded by a lock for all mutations and lookups.
- Publish iterates over a snapshot to avoid issues if handlers subscribe/unsubscribe during delivery.
- After unsubscription, empty handler lists are removed to keep memory usage small.

## NuGet preprocessor symbol
When you add the EventCoalTrain NuGet package, the compile symbol `EVENTCOALTRAIN` is automatically defined in your consuming project via a `buildTransitive` MSBuild props.

Example:
```csharp
#if EVENTCOALTRAIN
Console.WriteLine("EventCoalTrain is referenced.");
#endif
```
Works for direct and transitive references. No project changes required.

### Unity without NuGet
If you drop the DLL directly into Unity (not via NuGet), Unity won’t import MSBuild props. Define the symbol manually in Player Settings > Scripting Define Symbols: `EVENTCOALTRAIN`.

### Opting out
Override `DefineConstants` in your project, or add a props after package imports to remove the symbol if needed.

### What’s inside the package
The package ships `buildTransitive/EventCoalTrain.props` with:
```xml
<Project>
    <PropertyGroup>
        <DefineConstants>$(DefineConstants);EVENTCOALTRAIN</DefineConstants>
    </PropertyGroup>
</Project>
```

## License
MIT
