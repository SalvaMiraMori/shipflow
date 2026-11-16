using Core.Specification;
using Core.Specification.Builder;

namespace Core.Tests.Specification;

public class SpecificationTests
{
    private sealed record Item(int Number, string Name);

    private sealed class EvenItemsSpecification : Specification<Item>
    {
        public EvenItemsSpecification()
        {
            Query.Where(item => item.Number % 2 == 0)
                .OrderByDescending(item => item.Number)
                .TagWith("EvenItems");
        }
    }

    private static readonly Item[] _items = [new(1, "one"), new(2, "two"), new(3, "three"), new(4, "four")];

    [Fact]
    public void Evaluate_InMemory_AppliesCriteriaAndOrder()
    {
        var result = new EvenItemsSpecification().Evaluate(_items).ToList();

        result.Select(item => item.Number).ShouldBe(new[] { 4, 2 });
    }

    [Fact]
    public void TagWith_StoresTagInItems()
    {
        new EvenItemsSpecification().Items[QueryTagBuilderExtensions.TagKey].ShouldBe("EvenItems");
    }

    [Fact]
    public void IsSatisfiedBy_ChecksCriteria()
    {
        var specification = new EvenItemsSpecification();

        specification.IsSatisfiedBy(new Item(2, "two")).ShouldBeTrue();
        specification.IsSatisfiedBy(new Item(3, "three")).ShouldBeFalse();
    }
}
