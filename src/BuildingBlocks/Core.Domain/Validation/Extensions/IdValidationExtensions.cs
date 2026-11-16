using System.Linq.Expressions;
using Core.Domain.Errors;
using FluentValidation;

namespace Core.Domain.Validation.Extensions;

public static class IdValidationExtensions
{
    /// <summary>
    /// The value must be a usable identifier: not null, not <see cref="Guid.Empty"/>, not blank, not &lt;= 0.
    /// </summary>
    public static IRuleBuilderOptions<TEntity, TProperty> AddIdValidation<TEntity, TProperty>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        string propertyName)
    {
        var error = ErrorTemplates.InvalidGuid(propertyName);
        return validator.RuleFor(propertyExpression)
            .Must(id => ValidationHelpers.BeAValidId(id))
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }

    /// <summary>
    /// Generic required-field rule (not null, not empty).
    /// </summary>
    public static IRuleBuilderOptions<TEntity, TProperty> AddRequiredValidation<TEntity, TProperty>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        string propertyName)
    {
        var error = ErrorTemplates.NullOrEmpty(propertyName);
        return validator.RuleFor(propertyExpression)
            .NotEmpty()
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }
}
