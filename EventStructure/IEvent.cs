#define EVENTCOALTRAIN
namespace EventCoalTrain.EventStructure;

/// <summary>
/// Represents a generic event with a name.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Gets the name of the event.
    /// </summary>
    string Name { get; }
}