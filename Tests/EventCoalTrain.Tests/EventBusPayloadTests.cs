using EventCoalTrain.EventHandling;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.Tests;

public class EventBusPayloadTests
{
    private static readonly EventKey<ScoreIncreased> Key = EventKey<ScoreIncreased>.Of($"ScoreIncreased_{nameof(EventBusPayloadTests)}_{Guid.NewGuid()}");

    private record struct ScoreIncreased(int Amount, string Source);
    
    private static readonly Packet<ScoreIncreased> ScoreIncreasedPacket = new(Key);

    public EventBusPayloadTests()
    {
        EventBus.Clear();
    }

    [Fact]
    public void Subscribe_and_Publish_delivers_payload()
    {
        var received = default(ScoreIncreased?);
        using var sub = EventBus.Instance.Subscribe(ScoreIncreasedPacket, p => received = p);

        var payload = new ScoreIncreased(5, "UnitTest");
        EventBus.Publish(ScoreIncreasedPacket, payload);

        Assert.True(received.HasValue);
        Assert.Equal(payload.Amount, received!.Value.Amount);
        Assert.Equal(payload.Source, received.Value.Source);
        Assert.True(EventBus.HasSubscribers(Key));
        Assert.Equal(1, EventBus.Count(Key));
    }

    [Fact]
    public void Disposing_subscription_stops_delivery()
    {
        var count = 0;
        var sub = EventBus.Instance.Subscribe(ScoreIncreasedPacket, _ => Interlocked.Increment(ref count));
        sub.Dispose();

        EventBus.Publish(ScoreIncreasedPacket, new ScoreIncreased(1, "x"));
        Assert.Equal(0, count);
        Assert.False(EventBus.HasSubscribers(Key));
        Assert.Equal(0, EventBus.Count(Key));
    }

    [Fact]
    public void UnsubscribeAll_clears_key_subscribers()
    {
        using var s1 = EventBus.Instance.Subscribe(ScoreIncreasedPacket, _ => { });
        using var s2 = EventBus.Instance.Subscribe(ScoreIncreasedPacket, _ => { });
        Assert.Equal(2, EventBus.Count(Key));

        EventBus.UnsubscribeAll(Key);
        Assert.Equal(0, EventBus.Count(Key));
        Assert.False(EventBus.HasSubscribers(Key));
    }
}

