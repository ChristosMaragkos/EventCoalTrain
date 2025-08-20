using EventCoalTrain.EventStructure;

namespace EventCoalTrain.Tests;

public class EventKeyTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\r\n")]
    public void Of_throws_on_null_or_whitespace(string name)
    {
        Assert.Throws<ArgumentException>(() => EventKey<int>.Of(name));
    }

    [Fact]
    public void Of_throws_on_duplicate_name_across_types()
    {
        var name = $"Dup_{Guid.NewGuid()}";
        var k1 = EventKey<int>.Of(name);
        Assert.Equal(name, k1.Name);
        Assert.Throws<InvalidOperationException>(() => EventKey<string>.Of(name));
    }

    [Fact]
    public void Equals_and_hashcode_respect_type_and_name()
    {
        var name = $"Eq_{Guid.NewGuid()}";
        var k1 = EventKey<int>.Of(name);
        // Same type and name would throw on creation; simulate equality semantics by reference equality
        Assert.True(k1.Equals(k1));

        var k2 = EventKey<int>.Of($"{name}_other");
        Assert.False(k1.Equals(k2));

        // Different payload type can't be equal (different generic type)
        var otherName = $"Eq_{Guid.NewGuid()}";
        var k3 = EventKey<string>.Of(otherName);
        Assert.False(k1.Equals((object)k3));

        Assert.NotEqual(k1.GetHashCode(), k2.GetHashCode());
    }

    [Fact]
    public void ToString_contains_type_and_name()
    {
        var name = $"TS_{Guid.NewGuid()}";
        var k = EventKey<double>.Of(name);
        var s = k.ToString();
        Assert.Contains("EventKey<Double>", s);
        Assert.Contains(name, s);
    }
}

