using EventCoalTrain.EventHandling;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.Tests;

public class SnapshotBehaviorTests
{
    [Fact]
    public void Subscribing_during_publish_does_not_affect_current_dispatch()
    {
        var key = EventKey<int>.Of($"Snap_{Guid.NewGuid()}");
        var packet = new Packet<int>(key);
        var aCalls = 0;
        var bCalls = 0;

        Action<int>? b = null;
        Action<int> a = _ =>
        {
            Interlocked.Increment(ref aCalls);
            // Subscribe B during A's first call
            if (b != null) return;
            b = _ => Interlocked.Increment(ref bCalls);
            EventBus.Instance.Subscribe(packet, b);
        };

        using var subA = EventBus.Instance.Subscribe(packet, a);

        // First publish: A runs, B gets subscribed, but shouldn't run this time
        EventBus.Publish(packet, 1);
        Assert.Equal(1, aCalls);
        Assert.Equal(0, bCalls);

        // Second publish: both A and B should run
        EventBus.Publish(packet, 2);
        Assert.Equal(2, aCalls);
        Assert.Equal(1, bCalls);
    }

    [Fact]
    public void Unsubscribing_during_publish_affects_future_publishes_only()
    {
        var key = EventKey<Unit>.Of($"Unsub_{Guid.NewGuid()}");
        var notif = new Notification(key);
        var calls = 0;
        Action removeSelf = null!;

        var sub = EventBus.Instance.Subscribe(notif, Handler);
        removeSelf = sub.Dispose;

        // First publish: handler invoked once; unsubscribes itself
        EventBus.Publish(notif);
        Assert.Equal(1, calls);

        // Second publish: should not be invoked again
        EventBus.Publish(notif);
        Assert.Equal(1, calls);
        return;

        void Handler()
        {
            Interlocked.Increment(ref calls);
            // Remove this handler during its first invocation
            removeSelf.Invoke();
        }
    }
}

