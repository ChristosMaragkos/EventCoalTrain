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

    [Fact]
    public void Legacy_static_Subscribe_and_Unsubscribe_notification_work()
    {
        var called = 0;
        Action handler = () => called++;
        EventBus.Subscribe(Notification, handler);

        EventBus.Publish(Notification);
        Assert.Equal(1, called);

        EventBus.Unsubscribe(Notification, handler);
        Assert.False(EventBus.HasSubscribers(_key));
    }
}

