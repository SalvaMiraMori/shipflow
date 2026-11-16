using System.Linq.Expressions;
using Core.Domain.Errors;
using FluentValidation;

namespace Core.Domain.Validation.Extensions;

public static class ValueValidationExtensions
{
    public static IRuleBuilderOptions<TEntity, decimal> AddGreaterThanValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, decimal>> propertyExpression,
        decimal value,
        string propertyName)
    {
        var error = ErrorTemplates.GreaterThanValue(propertyName, value);
        return validator.RuleFor(propertyExpression)
            .GreaterThan(value)
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }

    public static IRuleBuilderOptions<TEntity, decimal> AddMinValueValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, decimal>> propertyExpression,
        decimal minValue,
        string propertyName)
    {
        var error = ErrorTemplates.GreaterThanOrEqualValue(propertyName, minValue);
        return validator.RuleFor(propertyExpression)
            .GreaterThanOrEqualTo(minValue)
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }

    public static IRuleBuilderOptions<TEntity, decimal> AddMaxValueValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, decimal>> propertyExpression,
        decimal maxValue,
        string propertyName)
    {
        var error = ErrorTemplates.LessThanOrEqualValue(propertyName, maxValue);
        return validator.RuleFor(propertyExpression)
            .LessThanOrEqualTo(maxValue)
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }

    public static IRuleBuilderOptions<TEntity, decimal> AddValueRangeValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, decimal>> propertyExpression,
        decimal minValue,
        decimal maxValue,
        string propertyName)
    {
        var minError = ErrorTemplates.GreaterThanOrEqualValue(propertyName, minValue);
        var maxError = ErrorTemplates.LessThanOrEqualValue(propertyName, maxValue);
        return validator.RuleFor(propertyExpression)
            .GreaterThanOrEqualTo(minValue)
            .WithErrorCode(minError.Code)
            .WithMessage(minError.Description)
            .LessThanOrEqualTo(maxValue)
            .WithErrorCode(maxError.Code)
            .WithMessage(maxError.Description);
    }
}
