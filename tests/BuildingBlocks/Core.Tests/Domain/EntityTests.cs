namespace Core.Tests.Domain;

public class EntityTests
{
    [Fact]
    public void Equals_SameTypeAndId_ReturnsTrue()
    {
        var id = Guid.NewGuid();

        var first = new TestAggregate(new TestId(id), "a");
        var second = new TestAggregate(new TestId(id), "b");

        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }

    [Fact]
    public void GetHashCode_AggregateCreatedWithId_DoesNotThrow()
    {
        // Regression: the old AggregateRoot hid BaseEntity.Id, which stayed null, so GetHashCode threw.
        var aggregate = new TestAggregate(new TestId(Guid.NewGuid()), "a");

        Should.NotThrow(() => aggregate.GetHashCode());
        aggregate.Id.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_RaisesDomainEvent_ClearRemovesIt()
    {
        var aggregate = new TestAggregate(new TestId(Guid.NewGuid()), "a");

        aggregate.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<TestCreated>();

        aggregate.ClearDomainEvents();
        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void MarkAsDeleted_SetsSoftDeleteFields()
    {
        var aggregate = new TestAggregate(new TestId(Guid.NewGuid()), "a");
        var deletedAt = DateTimeOffset.UtcNow;

        aggregate.MarkAsDeleted(deletedAt, "user-1");

        aggregate.IsDeleted.ShouldBeTrue();
        aggregate.Deleted.ShouldBe(deletedAt);
        aggregate.DeletedBy.ShouldBe("user-1");
    }
}
