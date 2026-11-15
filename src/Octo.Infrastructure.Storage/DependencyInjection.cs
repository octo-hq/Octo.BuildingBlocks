using Microsoft.Extensions.DependencyInjection;
using Octo.Infrastructure.Storage.Services;

namespace Octo.Infrastructure.Storage;

public static class DependencyInjection
{
    public static IServiceCollection AddStorageInfrastructure<TStorageProvider>(this IServiceCollection services)
        where TStorageProvider : class, IStorageService
    {
        // Registra a implementação personalizada fornecida externamente
        services.AddScoped<IStorageService, TStorageProvider>();
        return services;
    }

    public static IServiceCollection AddStorageInfrastructure(this IServiceCollection services)
    {
        // Registra o LocalStorageProvider como implementação padrão de IStorageService
        services.AddScoped<IStorageService, LocalStorageProvider>();
        return services;
    }
}