using Microsoft.EntityFrameworkCore;
using Octo.Core.Domain;
using Octo.Infrastructure.Tenant.Services;

namespace Octo.Infrastructure.Tenant.Persistence;

public static class TenantDbContextExtensions
{
    public static void ApplyTenantConfiguration(ModelBuilder modelBuilder, ITenantProvider tenantProvider)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Verifica se a entidade implementa IMultiTenantEntity
            if (typeof(IMultiTenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");

                // Expressão: e.TenantId == tenantProvider.TenantId
                var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, nameof(IMultiTenantEntity.TenantId));

                var tenantIdVal = System.Linq.Expressions.Expression.Constant(tenantProvider.TenantId ?? Guid.Empty);
                var comparison = System.Linq.Expressions.Expression.Equal(propertyAccess, tenantIdVal);

                var lambda = System.Linq.Expressions.Expression.Lambda(comparison, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public static void HandleTenantSavingChanges(DbContext context, ITenantProvider tenantProvider)
    {
        foreach (var entry in context.ChangeTracker.Entries<IMultiTenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (!tenantProvider.IsTenantSet)
                {
                    throw new InvalidOperationException("Não é possível salvar uma entidade multitenant sem um Tenant ativo.");
                }

                // Atribui automaticamente o TenantId da requisição corrente na entidade
                var property = entry.Property(e => e.TenantId);
                if (property.CurrentValue == Guid.Empty || property.CurrentValue == null)
                {
                    property.CurrentValue = tenantProvider.TenantId!.Value;
                }
            }
        }
    }
}