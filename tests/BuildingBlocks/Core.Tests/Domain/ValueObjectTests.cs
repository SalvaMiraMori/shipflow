namespace Core.Tests.Domain;

public class ValueObjectTests
{
    [Fact]
    public void Equals_SameComponents_ReturnsTrue()
    {
        new Pair("a", "b").ShouldBe(new Pair("a", "b"));
        (new Pair("a", "b") == new Pair("a", "b")).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SwappedComponents_AreDifferent()
    {
        // The old XOR-based hash returned the same value for (a, b) and (b, a).
        new Pair("a", "b").GetHashCode().ShouldNotBe(new Pair("b", "a").GetHashCode());
    }

    [Fact]
    public void GetHashCode_NoComponents_DoesNotThrow()
    {
        // The old Aggregate((x, y) => x ^ y) threw InvalidOperationException on an empty sequence.
        Should.NotThrow(() => new EmptyValueObject().GetHashCode());
    }
}
