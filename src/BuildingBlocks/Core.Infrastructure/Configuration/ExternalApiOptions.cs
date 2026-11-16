using System.ComponentModel.DataAnnotations;

namespace Core.Infrastructure.Configuration;

/// <summary>
/// Settings of an external HTTP API (carrier, payment provider...). Bound from <c>ExternalApis:{Name}</c> and
/// validated on start-up. API keys come from user-secrets / Key Vault, never from appsettings.
/// </summary>
public sealed class ExternalApiOptions
{
    public const string SectionName = "ExternalApis";

    [Required]
    [Url]
    public string BaseUrl { get; init; } = string.Empty;

    public string? ApiKey { get; init; }

    [Range(1, 300)]
    public int TimeoutSeconds { get; init; } = 30;
}
