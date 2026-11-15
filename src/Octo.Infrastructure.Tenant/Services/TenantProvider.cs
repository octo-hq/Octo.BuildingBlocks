using Microsoft.AspNetCore.Http;

namespace Octo.Infrastructure.Tenant.Services;

public interface ITenantProvider
{
    Guid? TenantId { get; }
    string? TenantCode { get; }
    bool IsTenantSet { get; }
}

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? TenantId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null) return null;

            // Tenta obter do Header HTTP customizado (X-Tenant-Id)
            if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) &&
                Guid.TryParse(tenantHeader, out var parsedId))
            {
                return parsedId;
            }

            // Opcional: Tenta obter das Claims do Token JWT autenticado
            var tenantClaim = context.User?.FindFirst("tenant_id")?.Value;
            if (!string.IsNullOrEmpty(tenantClaim) && Guid.TryParse(tenantClaim, out var claimId))
            {
                return claimId;
            }

            return null;
        }
    }

    public string? TenantCode
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Request.Headers["X-Tenant-Code"].FirstOrDefault()
                   ?? context?.User?.FindFirst("tenant_code")?.Value;
        }
    }

    public bool IsTenantSet => TenantId.HasValue && TenantId.Value != Guid.Empty;
}