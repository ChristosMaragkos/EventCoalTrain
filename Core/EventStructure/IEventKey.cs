namespace EventCoalTrain.EventStructure
{
    /// <summary>
    /// Represents a key for an event.
    /// An event key is used to uniquely identify an event within the event system.
    /// It contains the name of the event, which is used for subscribing and publishing events.
    /// </summary>
    public interface IEventKey
    {
        /// <summary>
        /// Gets the name of the event associated with this key.
        /// The name is used to identify the event when subscribing or publishing.
        /// </summary>
        public string Name { get; }
    }
}