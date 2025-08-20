using EventCoalTrain.EventHandling;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.Tests;

public class EventBusErrorTests
{
    private static readonly EventKey<int> Key = EventKey<int>.Of($"Err_{Guid.NewGuid()}");

    private static readonly Packet<int> IntegerPacket = new(Key);
    
    public EventBusErrorTests()
    {
        EventBus.Clear();
    }

    [Fact]
    public void OnPublishError_is_raised_and_other_handlers_continue()
    {
        Exception? captured = null;
        IEventKey? capturedKey = null;
        Delegate? capturedDelegate = null;

        Action<Exception, IEventKey, Delegate> handler = (ex, key, del) =>
        {
            captured = ex;
            capturedKey = key;
            capturedDelegate = del;
        };

        EventBus.OnPublishError += handler;
        try
        {
            var okCount = 0;
            void Throws(int _) => throw new InvalidOperationException("boom");
            void Ok(int _) => Interlocked.Increment(ref okCount);

            using var s1 = EventBus.Instance.Subscribe(IntegerPacket, Throws);
            using var s2 = EventBus.Instance.Subscribe(IntegerPacket, Ok);

            EventBus.Publish(IntegerPacket, 42);

            Assert.IsType<InvalidOperationException>(captured);
            Assert.Equal(Key.Name, capturedKey!.Name);
            Assert.NotNull(capturedDelegate);
            Assert.Equal(1, okCount); // other handler still ran
        }
        finally
        {
            EventBus.OnPublishError -= handler;
        }
    }
}
