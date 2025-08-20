using EventCoalTrain.EventHandling;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.Tests;

public class PacketDescriptorTests
{
    private readonly record struct Ping(string From);
    private static readonly EventKey<Ping> PingKey = EventKey<Ping>.Of($"Ping_{nameof(PacketDescriptorTests)}_{Guid.NewGuid()}");
    private static readonly Packet<Ping> PingPacket = new(PingKey);

    public PacketDescriptorTests()
    {
        EventBus.Clear();
    }

    [Fact]
    public void Publish_with_descriptor_delivers_payload()
    {
        Ping? got = null;
        using var _ = EventBus.Instance.Subscribe(PingPacket, p => got = p);

        var payload = new Ping("tester");
        EventBus.Publish(PingPacket, payload);

        Assert.True(got.HasValue);
        Assert.Equal(payload.From, got!.Value.From);
    }

    [Fact]
    public void Publish_with_key_and_payload_delivers_payload()
    {
        Ping? got = null;
        using var _ = EventBus.Instance.Subscribe(PingPacket, p => got = p);

        var payload = new Ping("keyPublish");
        EventBus.Publish(PingPacket, payload);

        Assert.True(got.HasValue);
        Assert.Equal(payload.From, got!.Value.From);
    }
}

