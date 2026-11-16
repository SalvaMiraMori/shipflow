using Core.Domain.Abstractions;
using Core.Domain.Events;

namespace Core.Tests.Domain;

public sealed class TestId : AggregateRootId<Guid>
{
    public TestId(Guid value) => Value = value;

    public override Guid Value { get; protected set; }
}

public sealed record TestCreated(Guid Id) : DomainEvent;

public sealed class TestAggregate : AggregateRoot<TestId, Guid>
{
    public TestAggregate(TestId id, string name)
        : base(id)
    {
        Name = name;
        RaiseDomainEvent(new TestCreated(id.Value));
    }

    public string Name { get; }
}

public sealed class Color : Enumeration<Color>
{
    public static readonly Color Red = new(1, nameof(Red));
    public static readonly Color Green = new(2, nameof(Green));

    private Color(int value, string name)
        : base(value, name)
    {
    }
}

public sealed class EmptyValueObject : ValueObject
{
    protected override IEnumerable<object?> GetEqualityComponents() => [];
}

public sealed class Pair(string left, string right) : ValueObject
{
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return left;
        yield return right;
    }
}
