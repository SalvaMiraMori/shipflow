namespace Core.Tests.Domain;

public class EnumerationTests
{
    [Fact]
    public void FromValue_KnownValue_ReturnsOption()
    {
        Color.FromValue(2).Value.ShouldBe(Color.Green);
    }

    [Fact]
    public void FromValue_UnknownValue_ReturnsValidationError()
    {
        var result = Color.FromValue(99);

        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Validation);
    }

    [Fact]
    public void FromName_IgnoresCase()
    {
        Color.FromName("red").Value.ShouldBe(Color.Red);
    }

    [Fact]
    public void List_ContainsAllOptions()
    {
        Color.List.Count.ShouldBe(2);
        Color.MinValue.ShouldBe(1);
        Color.MaxValue.ShouldBe(2);
    }
}
