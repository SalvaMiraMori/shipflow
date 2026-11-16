using System.Globalization;
using Core.Domain.Extensions;
using ErrorOr;

namespace Core.Domain.Errors;

/// <summary>
/// Reusable error factories with consistent codes (<c>{Entity}.{Reason}</c>) and messages.
/// Moved here from each service's Domain project (EcoGreen kept its own copy) so every service reports errors the same way.
/// Service-specific errors live in <c>DomainErrors.{Entity}</c> partial classes and are built from these templates.
/// </summary>
public static class ErrorTemplates
{
    // ---- Not found / persistence -------------------------------------------------------------------------------
    public static Error NotFound(string entity, object id) =>
        Error.NotFound($"{entity}.NotFound", Format("The {0} with the identifier {1} was not found.", entity, id));

    public static Error NotFoundByField(string entity, string fieldName, object? fieldValue) =>
        Error.NotFound($"{entity}.NotFound", Format("The {0} with the {1} {2} was not found.", entity, fieldName.ToReadableString(), fieldValue));

    public static Error NotFoundList(string entities) =>
        Error.NotFound($"{entities}.NotFound", Format("{0} data not found.", entities));

    public static Error CannotBeCreated(string entity) =>
        Error.Failure($"{entity}.CannotBeCreated", Format("{0} data not saved due to a problem when creating.", entity));

    public static Error CannotBeUpdated(string entity) =>
        Error.Failure($"{entity}.CannotBeUpdated", Format("{0} data not saved due to a problem when updating.", entity));

    public static Error CannotBeDeleted(string entity) =>
        Error.Failure($"{entity}.CannotBeDeleted", Format("{0} data not saved due to a problem when deleting.", entity));

    public static Error FieldConflict(string entity, string field, object? value) =>
        Error.Conflict($"{entity}.{field}.Conflict", Format("This {0} {1} '{2}' already exists.", entity, field.ToReadableString(), value));

    // ---- Authorization ----------------------------------------------------------------------------------------
    public static Error InvalidCredentials(string fieldName) =>
        Error.Unauthorized($"{fieldName}.InvalidCredentials", "Invalid credentials.");

    public static Error NotOwnedByTenant(string entity) =>
        Error.Forbidden($"{entity}.NotOwnedByTenant", Format("The {0} does not belong to the current tenant.", entity));

    // ---- Generic validation -----------------------------------------------------------------------------------
    public static Error NullOrEmpty(string fieldName) =>
        Error.Validation($"{fieldName}.NullOrEmpty", Format("The {0} is required.", fieldName.ToReadableString()));

    public static Error InvalidGuid(string fieldName) =>
        Error.Validation($"{fieldName}.InvalidGuid", Format("{0} must be a valid, non-empty identifier.", fieldName));

    public static Error InvalidValue<T>(string fieldName, T value) =>
        Error.Validation($"{fieldName}.InvalidValue", Format("The {0} value '{1}' is not valid.", fieldName.ToReadableString(), value));

    public static Error InvalidFormat(string fieldName) =>
        Error.Validation($"{fieldName}.InvalidFormat", Format("{0} format is invalid.", fieldName.ToReadableString()));

    public static Error InvalidString(string fieldName) =>
        Error.Validation($"{fieldName}.InvalidString", Format("{0} must be a non-empty text.", fieldName.ToReadableString()));

    public static Error InvalidEmailFormat(string fieldName) =>
        Error.Validation($"{fieldName}.InvalidEmailFormat", Format("{0} must be a valid email address.", fieldName.ToReadableString()));

    public static Error InvalidOperation(string entity, string reason) =>
        Error.Validation($"{entity}.InvalidOperation", Format("The operation is invalid for {0}: {1}", entity, reason));

    public static Error RequiredWhen(string fieldName, string condition) =>
        Error.Validation($"{fieldName}.RequiredWhen", Format("{0} is required when {1}.", fieldName.ToReadableString(), condition));

    // ---- Length -----------------------------------------------------------------------------------------------
    public static Error InvalidLength(string fieldName, int length) =>
        Error.Validation($"{fieldName}.InvalidLength", Format("The {0} requires a length of {1}.", fieldName.ToReadableString(), length));

    public static Error LengthOutOfRange(string fieldName, int minLength, int maxLength) =>
        Error.Validation($"{fieldName}.LengthOutOfRange", Format("The length of {0} must be between {1} and {2}.", fieldName.ToReadableString(), minLength, maxLength));

    public static Error ShorterThanAllowed(string fieldName, int minLength) =>
        Error.Validation($"{fieldName}.ShorterThanAllowed", Format("The {0} must have at least {1} characters.", fieldName.ToReadableString(), minLength));

    public static Error LongerThanAllowed(string fieldName, int maxLength) =>
        Error.Validation($"{fieldName}.LongerThanAllowed", Format("The {0} must have at most {1} characters.", fieldName.ToReadableString(), maxLength));

    // ---- Numbers ----------------------------------------------------------------------------------------------
    public static Error GreaterThanValue(string fieldName, decimal value) =>
        Error.Validation($"{fieldName}.GreaterThan", Format("{0} must be greater than {1}.", fieldName.ToReadableString(), value));

    public static Error GreaterThanOrEqualValue(string fieldName, decimal value) =>
        Error.Validation($"{fieldName}.GreaterThanOrEqual", Format("{0} must be greater than or equal to {1}.", fieldName.ToReadableString(), value));

    public static Error LessThanOrEqualValue(string fieldName, decimal value) =>
        Error.Validation($"{fieldName}.LessThanOrEqual", Format("{0} must be less than or equal to {1}.", fieldName.ToReadableString(), value));

    // ---- Dates ------------------------------------------------------------------------------------------------
    public static Error DateBeforeAnotherDate(string dateBeforeFieldName, string dateAfterFieldName) =>
        Error.Validation($"{dateBeforeFieldName}.After{dateAfterFieldName}", Format("{0} must be prior to {1}.", dateBeforeFieldName, dateAfterFieldName));

    public static Error DateMustBeInThePast(string fieldName) =>
        Error.Validation($"{fieldName}.DateMustBeInThePast", Format("{0} must be in the past.", fieldName.ToReadableString()));

    public static Error DateMustBeInTheFuture(string fieldName) =>
        Error.Validation($"{fieldName}.DateMustBeInTheFuture", Format("{0} must be in the future.", fieldName.ToReadableString()));

    private static string Format(string template, params object?[] args) =>
        string.Format(CultureInfo.InvariantCulture, template, args);
}
