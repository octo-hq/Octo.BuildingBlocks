using Microsoft.Extensions.DependencyInjection;
using Octo.Infrastructure.Tenant.Services;

namespace Octo.Infrastructure.Tenant;

public static class DependencyInjection
{
    public static IServiceCollection AddTenantInfrastructure(this IServiceCollection services)
    {        
        services.AddScoped<ITenantProvider, TenantProvider>();
        return services;
    }
}