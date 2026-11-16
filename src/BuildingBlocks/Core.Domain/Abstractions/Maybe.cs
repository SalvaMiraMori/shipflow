namespace Core.Domain.Abstractions;

/// <summary>
/// Represents a value that may or may not exist.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public sealed class Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T? _value;
    private readonly bool _hasValue;

    private Maybe(T? value)
    {
        _value = value;
        _hasValue = value is not null;
    }

    public static Maybe<T> None { get; } = new(default);

    public bool HasValue => _hasValue;

    public bool HasNoValue => !_hasValue;

    public T Value => _hasValue
        ? _value!
        : throw new InvalidOperationException("The value can not be accessed because it does not exist.");

    public static Maybe<T> From(T? value) => value is null ? None : new Maybe<T>(value);

    public static implicit operator Maybe<T>(T? value) => From(value);

    public T GetValueOrDefault(T defaultValue) => _hasValue ? _value! : defaultValue;

    public TOut Match<TOut>(Func<T, TOut> onValue, Func<TOut> onNone) => _hasValue ? onValue(_value!) : onNone();

    public bool Equals(Maybe<T>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (HasNoValue && other.HasNoValue)
        {
            return true;
        }

        return HasValue && other.HasValue && EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    public override bool Equals(object? obj) => obj is Maybe<T> other && Equals(other);

    public override int GetHashCode() => _hasValue ? EqualityComparer<T>.Default.GetHashCode(_value!) : 0;
}
