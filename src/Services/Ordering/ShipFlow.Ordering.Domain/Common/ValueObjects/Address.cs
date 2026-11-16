using Core.Domain.Abstractions;
using Core.Domain.Validation;
using ErrorOr;
using ShipFlow.Ordering.Domain.Common.Validation.Validators;

namespace ShipFlow.Ordering.Domain.Common.ValueObjects;

/// <summary>
/// A postal address. <see cref="CountryCode"/> is ISO 3166-1 alpha-2 (e.g. <c>ES</c>).
/// </summary>
public sealed class Address : ValueObject
{
    public const int StreetMaxLength = 200;
    public const int CityMaxLength = 100;
    public const int PostalCodeMaxLength = 20;
    public const int CountryCodeLength = 2;
    public const string CountryCodePattern = "^[A-Z]{2}$";

    private Address(string street, string city, string postalCode, string countryCode)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        CountryCode = countryCode;
    }

#pragma warning disable CS8618 // Required by EF Core.
    private Address()
    {
    }
#pragma warning restore CS8618

    public string Street { get; private set; }

    public string City { get; private set; }

    public string PostalCode { get; private set; }

    public string CountryCode { get; private set; }

    public static ErrorOr<Address> CreateNew(string? street, string? city, string? postalCode, string? countryCode) =>
        AddressValidator.Instance.ValidateToErrorOr(new Address(
            street?.Trim() ?? string.Empty,
            city?.Trim() ?? string.Empty,
            postalCode?.Trim() ?? string.Empty,
            countryCode?.Trim().ToUpperInvariant() ?? string.Empty));

    public override string ToString() => $"{Street}, {PostalCode} {City}, {CountryCode}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return CountryCode;
    }
}
