using System.Text.RegularExpressions;

namespace Core.Domain.Extensions;

public static partial class StringExtensions
{
    /// <summary>
    /// "DeliveryNumber" → "delivery number". Used to build readable validation messages.
    /// </summary>
    public static string ToReadableString(this string value) =>
        string.IsNullOrWhiteSpace(value) ? value : UpperCaseRegex().Replace(value, " $1").ToLowerInvariant();

    public static bool IsNullOrWhiteSpace([System.Diagnostics.CodeAnalysis.NotNullWhen(false)] this string? value) =>
        string.IsNullOrWhiteSpace(value);

    [GeneratedRegex("(\\B[A-Z])")]
    private static partial Regex UpperCaseRegex();
}
