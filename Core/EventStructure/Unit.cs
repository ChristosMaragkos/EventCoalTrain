namespace EventCoalTrain.EventStructure;

/// <summary>
/// Represents a type with a single value, similar to <c>void</c> but as a struct.
/// Used to indicate the absence of a payload for event keys.
/// </summary>
public readonly struct Unit
{
    /// <summary>
    /// The single value of the <see cref="Unit"/> type.
    /// </summary>
    public static readonly Unit Value = new();
}