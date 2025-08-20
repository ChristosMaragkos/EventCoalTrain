using EventCoalTrain.EventHandling;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.Tests;

public class EventBusNotificationTests
{
    private readonly EventKey<Unit> _key = EventKey<Unit>.Of($"Notify_{nameof(EventBusNotificationTests)}_{Guid.NewGuid()}");
    private Notification Notification => new(_key);

    public EventBusNotificationTests()
    {
        EventBus.Clear();
    }

    [Fact]
    public void Subscribe_and_Publish_notification_invokes_handler()
    {
        var called = 0;
        using var sub = EventBus.Instance.Subscribe(Notification, () => Interlocked.Increment(ref called));

        EventBus.Publish(Notification);
        Assert.Equal(1, called);
        Assert.True(EventBus.HasSubscribers(_key));
        Assert.Equal(1, EventBus.Count(_key));
    }
}

