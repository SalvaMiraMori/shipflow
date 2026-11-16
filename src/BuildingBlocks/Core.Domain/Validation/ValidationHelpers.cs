using System.Text.RegularExpressions;

namespace Core.Domain.Validation;

/// <summary>
/// Null-safe predicates used by the validator extensions. The previous helpers threw
/// <see cref="NullReferenceException"/> when a string was null; these return <c>false</c> instead.
/// </summary>
public static class ValidationHelpers
{
    public static bool BeAValidId<T>(T? id) => id switch
    {
        null => false,
        Guid guid => guid != Guid.Empty,
        string text => !string.IsNullOrWhiteSpace(text),
        int number => number > 0,
        long number => number > 0,
        _ => !EqualityComparer<T>.Default.Equals(id, default),
    };

    public static bool BeAValidString(string? value) => !string.IsNullOrWhiteSpace(value);

    public static bool BeAValidExactLength(string? value, int exactLength) => value?.Length == exactLength;

    public static bool BeAValidLengthRange(string? value, int minLength, int maxLength) =>
        value is not null && value.Length >= minLength && value.Length <= maxLength;

    public static bool BeAValidMinLength(string? value, int minLength) => value is not null && value.Length >= minLength;

    public static bool BeAValidMaxLength(string? value, int maxLength) => value is null || value.Length <= maxLength;

    public static bool BeAValidFormat(string? value, string pattern) =>
        value is not null && Regex.IsMatch(value, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(250));

    public static bool BeAValidDateTimeRange(DateTime startDate, DateTime endDate) => startDate < endDate;
}
