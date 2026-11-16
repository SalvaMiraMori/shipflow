namespace Core.Application.Services;

/// <summary>
/// The caller of the current request. <see cref="UserId"/> is used for auditing; <see cref="TenantId"/> for multi-tenancy.
/// </summary>
public interface ICurrentUserProvider
{
    string? UserId { get; }

    Guid? TenantId { get; }

    bool IsAuthenticated { get; }
}
