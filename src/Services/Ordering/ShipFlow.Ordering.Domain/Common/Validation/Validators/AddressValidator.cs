using Core.Domain.Validation.Extensions;
using FluentValidation;
using ShipFlow.Ordering.Domain.Common.ValueObjects;

namespace ShipFlow.Ordering.Domain.Common.Validation.Validators;

public sealed class AddressValidator : AbstractValidator<Address>
{
    public static readonly AddressValidator Instance = new();

    public AddressValidator()
    {
        this.AddLengthRangeValidation(address => address.Street, 1, Address.StreetMaxLength, nameof(Address.Street));
        this.AddLengthRangeValidation(address => address.City, 1, Address.CityMaxLength, nameof(Address.City));
        this.AddLengthRangeValidation(address => address.PostalCode, 1, Address.PostalCodeMaxLength, nameof(Address.PostalCode));
        this.AddRegexValidation(address => address.CountryCode, Address.CountryCodePattern, nameof(Address.CountryCode));
    }
}
