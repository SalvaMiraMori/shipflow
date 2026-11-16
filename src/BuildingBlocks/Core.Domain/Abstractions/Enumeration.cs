using System.Reflection;
using Core.Domain.Errors;
using ErrorOr;

namespace Core.Domain.Abstractions;

/// <summary>
/// Smart enum: a closed set of named values that can carry behaviour, persisted as its <see cref="Value"/>.
/// Replaces the two near-identical <c>Enumeration</c>/<c>EnumerationGeneric</c> classes of the previous Core.
/// </summary>
/// <typeparam name="TEnum">The enumeration type.</typeparam>
/// <typeparam name="TValue">The persisted value type.</typeparam>
public abstract class Enumeration<TEnum, TValue> : IEquatable<Enumeration<TEnum, TValue>>, IComparable<Enumeration<TEnum, TValue>>
    where TEnum : Enumeration<TEnum, TValue>
    where TValue : notnull, IComparable<TValue>
{
    private static readonly Lazy<Dictionary<TValue, TEnum>> _byValue =
        new(() => GetAllOptions().ToDictionary(item => item.Value));

    protected Enumeration(TValue value, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Value = value;
        Name = name;
    }

#pragma warning disable CS8618 // Required by EF Core.
    protected Enumeration()
    {
        Name = string.Empty;
    }
#pragma warning restore CS8618

    public static IReadOnlyCollection<TEnum> List => _byValue.Value.Values;

    public TValue Value { get; private set; }

    public string Name { get; private set; }

    public static Maybe<TEnum> TryFromValue(TValue value) =>
        _byValue.Value.TryGetValue(value, out var enumeration) ? Maybe<TEnum>.From(enumeration) : Maybe<TEnum>.None;

    public static Maybe<TEnum> TryFromName(string? name) =>
        Maybe<TEnum>.From(List.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase)));

    /// <summary>
    /// Returns the option with the given value or a validation error, ready to use in ErrorOr flows
    /// (EcoGreen re-implemented this in every enumeration).
    /// </summary>
    public static ErrorOr<TEnum> FromValue(TValue value) =>
        _byValue.Value.TryGetValue(value, out var enumeration)
            ? enumeration
            : ErrorTemplates.InvalidValue(typeof(TEnum).Name, value);

    public static ErrorOr<TEnum> FromName(string? name)
    {
        var maybe = TryFromName(name);
        return maybe.HasValue ? maybe.Value : ErrorTemplates.InvalidValue(typeof(TEnum).Name, name ?? string.Empty);
    }

    public static bool ContainsValue(TValue value) => _byValue.Value.ContainsKey(value);

    public static bool operator ==(Enumeration<TEnum, TValue>? left, Enumeration<TEnum, TValue>? right) => Equals(left, right);

    public static bool operator !=(Enumeration<TEnum, TValue>? left, Enumeration<TEnum, TValue>? right) => !Equals(left, right);

    public bool Equals(Enumeration<TEnum, TValue>? other) =>
        other is not null && GetType() == other.GetType() && EqualityComparer<TValue>.Default.Equals(Value, other.Value);

    public override bool Equals(object? obj) => obj is Enumeration<TEnum, TValue> other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public int CompareTo(Enumeration<TEnum, TValue>? other) => other is null ? 1 : Value.CompareTo(other.Value);

    public override string ToString() => Name;

    private static IEnumerable<TEnum> GetAllOptions()
    {
        var enumType = typeof(TEnum);

        return enumType.Assembly
            .GetTypes()
            .Where(enumType.IsAssignableFrom)
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            .Where(field => enumType.IsAssignableFrom(field.FieldType))
            .Select(field => (TEnum)field.GetValue(null)!)
            .Distinct();
    }
}

/// <summary>
/// Smart enum persisted as an <see cref="int"/>.
/// </summary>
public abstract class Enumeration<TEnum> : Enumeration<TEnum, int>
    where TEnum : Enumeration<TEnum>
{
    protected Enumeration(int value, string name)
        : base(value, name)
    {
    }

    protected Enumeration()
    {
    }

    public static int MinValue => List.Min(item => item.Value);

    public static int MaxValue => List.Max(item => item.Value);
}
