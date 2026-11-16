using System.Security.Claims;
using Core.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Identity;

/// <summary>
/// Reads the current user from the HTTP context claims (works with cookies, JWT bearer or Entra ID).
/// Generalises EcoGreen's <c>CurrentUserProvider</c> (user id from <c>NameIdentifier</c> or <c>sub</c>, tenant from <c>tenant_id</c>).
/// </summary>
public sealed class ClaimsCurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public const string TenantClaimType = "tenant_id";

    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub");

    public Guid? TenantId => Guid.TryParse(User?.FindFirstValue(TenantClaimType), out var tenantId) ? tenantId : null;
}
