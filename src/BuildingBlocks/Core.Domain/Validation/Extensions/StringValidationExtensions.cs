using System.Linq.Expressions;
using Core.Domain.Errors;
using FluentValidation;

namespace Core.Domain.Validation.Extensions;

public static class StringValidationExtensions
{
    public static IRuleBuilderOptions<TEntity, string?> AddStringValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, string?>> propertyExpression,
        string propertyName)
    {
        var error = ErrorTemplates.InvalidString(propertyName);
        return validator.RuleFor(propertyExpression)
            .Must(ValidationHelpers.BeAValidString)
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }

    public static IRuleBuilderOptions<TEntity, string?> AddExactLengthValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, string?>> propertyExpression,
        int exactLength,
        string propertyName)
    {
        var error = ErrorTemplates.InvalidLength(propertyName, exactLength);
        return validator.RuleFor(propertyExpression)
            .Must(value => ValidationHelpers.BeAValidExactLength(value, exactLength))
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }

    public static IRuleBuilderOptions<TEntity, string?> AddLengthRangeValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, string?>> propertyExpression,
        int minLength,
        int maxLength,
        string propertyName)
    {
        var error = ErrorTemplates.LengthOutOfRange(propertyName, minLength, maxLength);
        return validator.RuleFor(propertyExpression)
            .Must(value => ValidationHelpers.BeAValidLengthRange(value, minLength, maxLength))
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }

    public static IRuleBuilderOptions<TEntity, string?> AddMinLengthValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, string?>> propertyExpression,
        int minLength,
        string propertyName)
    {
        var error = ErrorTemplates.ShorterThanAllowed(propertyName, minLength);
        return validator.RuleFor(propertyExpression)
            .Must(value => ValidationHelpers.BeAValidMinLength(value, minLength))
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }

    public static IRuleBuilderOptions<TEntity, string?> AddMaxLengthValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, string?>> propertyExpression,
        int maxLength,
        string propertyName)
    {
        var error = ErrorTemplates.LongerThanAllowed(propertyName, maxLength);
        return validator.RuleFor(propertyExpression)
            .Must(value => ValidationHelpers.BeAValidMaxLength(value, maxLength))
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }

    public static IRuleBuilderOptions<TEntity, string?> AddRegexValidation<TEntity>(
        this AbstractValidator<TEntity> validator,
        Expression<Func<TEntity, string?>> propertyExpression,
        string regexPattern,
        string propertyName)
    {
        var error = ErrorTemplates.InvalidFormat(propertyName);
        return validator.RuleFor(propertyExpression)
            .Must(value => ValidationHelpers.BeAValidFormat(value, regexPattern))
            .WithErrorCode(error.Code)
            .WithMessage(error.Description);
    }
}
