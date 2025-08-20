using EventCoalTrain.EventHandling;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.Tests;

public class EventBusPayloadTests
{
    private readonly EventKey<ScoreIncreased> _key = EventKey<ScoreIncreased>.Of($"ScoreIncreased_{nameof(EventBusPayloadTests)}_{Guid.NewGuid()}");

    public record struct ScoreIncreased(int Amount, string Source);

    public EventBusPayloadTests()
    {
        EventBus.Clear();
    }

    [Fact]
    public void Subscribe_and_Publish_delivers_payload()
    {
        var received = default(ScoreIncreased?);
        using var sub = EventBus.Instance.Subscribe(_key, p => received = p);

        var payload = new ScoreIncreased(5, "UnitTest");
        EventBus.Publish(new Packet<ScoreIncreased>(_key, payload));

        Assert.True(received.HasValue);
        Assert.Equal(payload.Amount, received!.Value.Amount);
        Assert.Equal(payload.Source, received.Value.Source);
        Assert.True(EventBus.HasSubscribers(_key));
        Assert.Equal(1, EventBus.Count(_key));
    }

    [Fact]
    public void Disposing_subscription_stops_delivery()
    {
        var count = 0;
        var sub = EventBus.Instance.Subscribe(_key, _ => Interlocked.Increment(ref count));
        sub.Dispose();

        EventBus.Publish(new Packet<ScoreIncreased>(_key, new ScoreIncreased(1, "x")));
        Assert.Equal(0, count);
        Assert.False(EventBus.HasSubscribers(_key));
        Assert.Equal(0, EventBus.Count(_key));
    }

    [Fact]
    public void Legacy_static_Subscribe_and_Unsubscribe_work()
    {
        var packet = new Packet<ScoreIncreased>(_key, new ScoreIncreased(2, "legacy"));
        var count = 0;
        Action<ScoreIncreased> handler = _ => count++;

        EventBus.Subscribe(packet, handler);
        Assert.True(EventBus.HasSubscribers(_key));
        Assert.Equal(1, EventBus.Count(_key));

        EventBus.Publish(packet);
        Assert.Equal(1, count);

        EventBus.Unsubscribe(packet, handler);
        Assert.False(EventBus.HasSubscribers(_key));
    }

    [Fact]
    public void UnsubscribeAll_clears_key_subscribers()
    {
        using var s1 = EventBus.Instance.Subscribe(_key, _ => { });
        using var s2 = EventBus.Instance.Subscribe(_key, _ => { });
        Assert.Equal(2, EventBus.Count(_key));

        EventBus.UnsubscribeAll(_key);
        Assert.Equal(0, EventBus.Count(_key));
        Assert.False(EventBus.HasSubscribers(_key));
    }
}

