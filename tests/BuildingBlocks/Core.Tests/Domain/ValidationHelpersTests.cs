using Core.Domain.Validation;

namespace Core.Tests.Domain;

public class ValidationHelpersTests
{
    [Fact]
    public void LengthChecks_NullString_ReturnFalseInsteadOfThrowing()
    {
        ValidationHelpers.BeAValidLengthRange(null, 1, 10).ShouldBeFalse();
        ValidationHelpers.BeAValidMinLength(null, 1).ShouldBeFalse();
        ValidationHelpers.BeAValidFormat(null, "^a$").ShouldBeFalse();
    }

    [Fact]
    public void BeAValidId_EmptyValues_ReturnFalse()
    {
        ValidationHelpers.BeAValidId(Guid.Empty).ShouldBeFalse();
        ValidationHelpers.BeAValidId(" ").ShouldBeFalse();
        ValidationHelpers.BeAValidId(0).ShouldBeFalse();
        ValidationHelpers.BeAValidId(Guid.NewGuid()).ShouldBeTrue();
    }
}
