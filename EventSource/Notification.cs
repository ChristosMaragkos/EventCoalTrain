using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventSource;

/// <summary>
/// Represents a notification event with a specific key.
/// Notifications are used to signal events without requiring a payload.
/// They can be used for simple signaling purposes,
/// such as indicating that an action has occurred, like a button press.
/// </summary>
public sealed class Notification : IEvent
{
    /// <summary>
    /// Gets the event key associated with this notification.
    /// </summary>
    public EventKey<Unit> Key { get; }

    /// <summary>
    /// Gets the name of the event from the key.
    /// </summary>
    public string Name => Key.Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="Notification"/> class with the specified event key.
    /// </summary>
    /// <param name="key">The event key for this notification.</param>
    public Notification(EventKey<Unit> key)
    {
        Key = key;
    }
}