using ErrorOr;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Domain.Validation;

/// <summary>
/// Bridges FluentValidation and ErrorOr. Replaces the
/// <c>validationResult.Errors.ConvertAll(e =&gt; Error.Validation(...)).ToList()</c> block that EcoGreen repeated in
/// every value object and aggregate.
/// </summary>
public static class ValidatorExtensions
{
    public static List<Error> ToErrors(this ValidationResult result) =>
        result.Errors.ConvertAll(failure => Error.Validation(code: failure.ErrorCode, description: failure.ErrorMessage));

    /// <summary>
    /// Validates <paramref name="instance"/> and returns it, or the validation errors.
    /// </summary>
    public static ErrorOr<T> ValidateToErrorOr<T>(this IValidator<T> validator, T instance)
    {
        var result = validator.Validate(instance);
        return result.IsValid ? instance : result.ToErrors();
    }

    /// <summary>
    /// Validates <paramref name="value"/> and, when valid, builds the result with <paramref name="factory"/>.
    /// Typical use in a value object: <c>Validator.ValidateAndCreate(value, v =&gt; new Sku(v))</c>.
    /// </summary>
    public static ErrorOr<TResult> ValidateAndCreate<T, TResult>(this IValidator<T> validator, T value, Func<T, TResult> factory)
    {
        var result = validator.Validate(value);
        return result.IsValid ? factory(value) : result.ToErrors();
    }
}
